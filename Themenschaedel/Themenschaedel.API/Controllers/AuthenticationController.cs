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
                Token token = await _authenticationService.Login(user.Username, null, user.Password);

                if (token != null)
                {
                    LoginResponse response = new LoginResponse();
                    response.AccessToken = token.Value;
                    response.TokenType = "Bearer";
                    response.ValidUntil = token.ValidUntil;
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


        [HttpGet("verify/{verificationId}")]
        public async Task<string> Get(string verificationId)
        {
            bool isEmailVerified = await _authenticationService.VerifyEmail(verificationId);

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

            try
            {
                return new UserResponse(await _authenticationService.GetUser(token));
            }
            catch (TokenDoesNotExistException e)
            {
                return BadRequest("User token does not exist!");
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
            if (!await _databaseService.IsRegistrationEmailUnique(user.Email)) return BadRequest("A user with this email already exists!");
            if (!await _databaseService.IsRegistrationUsernameUnique(user.Username)) return BadRequest("A user with this username already exists!");

            bool wasUserRegistrationSuccessful = await _authenticationService.Register(user);

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
