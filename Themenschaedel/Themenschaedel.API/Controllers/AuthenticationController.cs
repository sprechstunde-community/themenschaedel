using System.Net.Http.Headers;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDatabaseService _databaseService;

        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationService authenticationService, IDatabaseService databaseService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _databaseService = databaseService;
        }

        [HttpDelete("logout")]
        public async Task<IActionResult> Delete()
        {
            string token = null;

            try
            {
                token = _authenticationService.GetValidToken(Request);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }

            bool logoutWorked = false;
            try
            {
                logoutWorked = await _authenticationService.LogoutAsync(token);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError("Error while trying to clear a singular token from the database. Error:\n" + e.Message);
            }

            if (logoutWorked)
            {
                return Ok();
            }

            return Problem();
        }

        [HttpDelete("logout/all")]
        public async Task<IActionResult> DeleteAll()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            string token = null;

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                token = headerValue.Parameter;
            }

            if (token == null) return Unauthorized();

            try
            {
                await _authenticationService.LogoutAllAsync(token);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError("Error while trying to delete all tokens. Error:\n" + e.Message);
                SentrySdk.CaptureException(e);
                return Problem();
            }

            return Ok();
        }

        // POST api/<EpisodesController>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Post([FromBody] UserLogin user)
        {
            if (user.Username.Length > 30) return BadRequest("Username is exceeding username lenght limit!");
            if (user.Username.Length == 0 || user.Username == "") return BadRequest("Username cannot be empty!");
            if (String.IsNullOrWhiteSpace(user.Username)) return BadRequest("Username cannot be empty!");
            if (String.IsNullOrWhiteSpace(user.Password)) return BadRequest("Invalid Password!");

            try
            {
                TokenExtended token = await _authenticationService.LoginAsync(user.Username, user.Password);

                if (token != null)
                {
                    LoginResponse response = new LoginResponse();
                    response.UserId = token.UserId;
                    response.AccessToken = token.Value;
                    response.TokenType = "Bearer";
                    response.ValidUntil = token.ValidUntil;
                    response.RefreshToken = token.RefreshToken;
                    return Ok(response);
                }
                else
                {
                    return Problem("An error occured, please try again or report the error.");
                }
            }
            catch (UserEmailNotVerifiedException e)
            {
                return BadRequest("User email not verified");
            }
            catch (PasswordOrEmailIncorrectException e)
            {
                return BadRequest("Username or password are incorrect!");
            }
            catch (UserDoesNotExistException e)
            {
                return BadRequest("User does not exist!");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while a user was trying to login. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }

        [HttpPost("refresh_token")]
        public async Task<ActionResult<LoginResponse>> PostRefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest.UserId == null) return BadRequest("UserId cannot be null!");
            if (refreshTokenRequest.UserId == 0) return BadRequest("UserId cannot be 0!");
            if (refreshTokenRequest.RefreshToken.Length == 0 || refreshTokenRequest.RefreshToken == "") return BadRequest("RefreshTokenAsync cannot be empty!");
            if (String.IsNullOrWhiteSpace(refreshTokenRequest.RefreshToken)) return BadRequest("RefreshTokenAsync cannot be empty!");

            try
            {
                return Ok(await _authenticationService.RefreshTokenAsync(refreshTokenRequest));
            }
            catch (UserDoesNotExistException e)
            {
                return BadRequest("User does not exist!");
            }
            catch (RefreshTokenDoesNotExist e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while a user was trying to login. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }


        [HttpGet("verify/{verificationId}")]
        public async Task<string> Get(string verificationId)
        {
            bool isEmailVerified = await _authenticationService.VerifyEmailAsync(verificationId);

            if (isEmailVerified)
            {
                return "Email verified.";
            }
            else
            {
                return "An error occured.";
            }
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> Get()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            string token = null;

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                token = headerValue.Parameter;
            }

            if (token == null) return Unauthorized();

            try
            {
                return new UserResponse(_authenticationService.GetUser(token));
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error trying to verify a user token. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }


        // POST api/<EpisodesController>
        [HttpPost("register")]
        public async Task<IActionResult> Post([FromBody] UserRegistration user)
        {
            if (user.Email == "") return BadRequest("Email is empty!");
            if (!IsValidEmailAddress(user.Email)) return BadRequest("The Email address is invalid!");
            if (user.Username.Length > 30) return BadRequest("Username is exceeding username lenght limit!");
            if (user.Username.Length == 0 || user.Username == "") return BadRequest("Username cannot be empty!");
            if (String.IsNullOrWhiteSpace(user.Username)) return BadRequest("Username cannot be empty!");
            if (user.Password.Length <= 8) return BadRequest("A password has to be more than 8 characters!");
            if (user.Password.Length > 127) return BadRequest("A password cannot be more than 127 characters!");
            if (!await _databaseService.IsRegistrationEmailUniqueAsync(user.Email)) return BadRequest("A user with this email already exists!");
            if (!await _databaseService.IsRegistrationUsernameUniqueAsync(user.Username)) return BadRequest("A user with this username already exists!");

            bool wasUserRegistrationSuccessful = await _authenticationService.RegisterAsync(user);

            if (wasUserRegistrationSuccessful)
            {
                return Ok("User created.");
            }
            else
            {
                return Problem("An error occured, please try again or report the error.");
            }
        }

        protected bool IsValidEmailAddress(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
