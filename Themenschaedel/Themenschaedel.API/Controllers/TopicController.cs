using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;

namespace Themenschaedel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly IAuthenticationService _authenticationService;
        public TopicController(IDatabaseService databaseService, IAuthenticationService authenticationService)
        {
            _databaseService = databaseService;
            _authenticationService = authenticationService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<TopicExtended>> GetAllTopics()
        {
            try
            {
                if (!await _authenticationService.CheckIfUserHasElivatedPermission(Request)) return Unauthorized();

                List<TopicExtended> episodes = await _databaseService.GetAllTopics();

                if (episodes.Count == 0)
                    return NoContent();

                return Ok(episodes);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return StatusCode(500, e.Message);
            }
        }
    }
}
