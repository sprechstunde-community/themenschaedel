using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Themenschaedel.API.Controllers
{
    [Route("api/vote")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        [HttpPost("{episodeId}")]
        public async Task<ActionResult<ClaimResponse>> Post(int episodeId)
        {
            if (id == 0) return BadRequest("Id cannot be 0!");
            if (id < 0) return BadRequest("Id cannot be smaller than 0!");

            try
            {
                User user = await _auth.GetUserFromValidToken(Request);
                EpisodeExtended episode = await _database.GetMinimalEpisodeAsync(id);
                if (episode.Claimed) return BadRequest("Episode is already claimed!");

                ClaimResponse response = await _claim.ClaimEpisodeAsync(episode, user.Id);

                return Ok(response);
            }
            catch (EpisodeUnclaimedButAlreadyHasTopcisException e)
            {
                return BadRequest("Episode is already claimed!");
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("23505") || e.Message.Contains("unique_user"))
                {
                    _logger.LogError($"User tried to claim episode wiht id {id}. But the user already claimed another episode.");
                    return BadRequest("User already has another claimed episode!");
                }

                _logger.LogError($"Error while trying to claim episode with id: {id}. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }
    }
}
