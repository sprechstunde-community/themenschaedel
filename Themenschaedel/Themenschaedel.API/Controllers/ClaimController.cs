using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly ILogger<ClaimController> _logger;

        public ClaimController(ILogger<ClaimController> logger)
        {
            _logger = logger;
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<ClaimResponse>> Post(int id) // id = EpisodeId
        {
            if (id == 0) return BadRequest("Id cannot be 0!");

            try
            {
                TokenExtended token = await _authenticationService.LoginAsync(user.Username, user.Password);

                if (token != null)
                {
                    LoginResponse response = new LoginResponse();
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
    }
}
