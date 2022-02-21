using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Shared.Models.Response;
using User = Themenschaedel.Shared.Models.User;

namespace Themenschaedel.API.Controllers
{
    [Route("api/vote")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly ILogger<VotesController> _logger;
        private readonly IDatabaseService _database;
        private readonly IAuthenticationService _auth;

        public VotesController(ILogger<VotesController> logger, IDatabaseService databaseService, IAuthenticationService auth)
        {
            _logger = logger;
            _database = databaseService;
            _auth = auth;
        }

        [HttpPost("{episodeId}")]
        public async Task<ActionResult> Post(int episodeId, [FromBody] VoteRequest request)
        {
            try
            {
                User user = await _auth.GetUserFromValidToken(Request);
                await _database.VoteForEpisode(request.Positive, episodeId, user.Id);
                return Ok();
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while trying to add vote for episode with id: {episodeId}. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }

        [HttpDelete("{episodeId}")]
        public async Task<ActionResult> Post(int episodeId)
        {
            try
            {
                User user = await _auth.GetUserFromValidToken(Request);
                await _database.DeleteVoteForEpisode(episodeId, user.Id);
                return Ok();
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while trying to delete vote for episode with id: {episodeId}. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }
    }
}
