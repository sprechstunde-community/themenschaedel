using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<TopicController> _logger;
        public TopicController(IDatabaseService databaseService, IAuthenticationService authenticationService, ILogger<TopicController> logger)
        {
            _databaseService = databaseService;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<ActionResult<TopicExtended>> GetAllTopics()
        {
            try
            {
                if (!await _authenticationService.CheckIfUserHasElivatedPermission(Request)) return Unauthorized();

                List<TopicExtended> episodes = await _databaseService.GetAllTopicsAsync();

                if (episodes.Count == 0)
                    return NoContent();

                return Ok(episodes);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError("Error while trying to get all topics. Error:\n" + e.Message);
                return Problem();
            }
        }

        // ToDo:
        //  Add endpoint to claim an episode under a /claim endpoint -> Make sure a user can only have one claim at a time! (there should be unique constraints on the database anyways lol) -> Default claim time = episode duration * 3 -> DONE!
        //  Add Post TopicExtended (so the client sends the topic and a list of subtopics) -> Make sure the user has claimed the episode he is posting to/editing
        //  Add endpoint for clients to extend claim time (for about 30 minutes)
        //  Add way for user/client to remove claim -> already saved stuff will be kept -> Maybe don't add, as users should not claim und unclaim "just to edit another episode", user has to commit to a claimed episode!
        //  Add way for admins to assign other users to abandoned claims
    }
}
