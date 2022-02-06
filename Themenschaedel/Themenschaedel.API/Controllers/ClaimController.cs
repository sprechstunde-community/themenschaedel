using FluentScheduler;
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
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly ILogger<ClaimController> _logger;
        private readonly IClaimService _claim;
        private readonly IAuthenticationService _auth;
        private readonly IDatabaseService _database;

        public ClaimController(ILogger<ClaimController> logger, IClaimService claimService, IAuthenticationService authenticationService, IDatabaseService databaseService)
        {
            _logger = logger;
            _claim = claimService;
            _auth = authenticationService;
            _database = databaseService;
        }

        [HttpGet]
        public async Task<ActionResult<Episode>> GetCurrentClaimedEpisode()
        {
            try
            {
                User user = await _auth.GetUserFromValidToken(Request);
                Episode claimedEpisode = await _claim.GetUserByClaimedEpisodeAsync(user.Id);
                if (claimedEpisode == null) return BadRequest("Currently there are no claimed episodes by this user.");
                return Ok(claimedEpisode);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Sequence contains no elements")) return BadRequest("Currently there are no claimed episodes by this user.");

                _logger.LogError($"Error while trying to get the episode the user currently has claimed. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<ClaimResponse>> Post(int id) // id = EpisodeId
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

        [HttpDelete("force/{id}")]
        public async Task<ActionResult> ForceDelete(int id) // id = EpisodeId
        {
            if (id == 0) return BadRequest("Id cannot be 0!");
            if (id < 0) return BadRequest("Id cannot be smaller than 0!");

            User user = null;

            try
            {
                user = await _auth.GetUserFromValidToken(Request);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error trying to get the user that is deleting the claim on episode with id {id}. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
                return Problem();
            }

            try
            {
                if (!await _auth.CheckIfUserHasElivatedPermission(Request)) return Unauthorized();

                await _claim.DeleteClaimByEpisodeIdAsync(id);

                return Ok();
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
                _logger.LogError($"Error while trying to delete the claim on episode with id: {id}. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }

        [HttpPost("submit")]
        public async Task<ActionResult> PostSubmit()
        {
            try
            {
                User user = await _auth.GetUserFromValidToken(Request);
                Episode claimedEpisode = await _claim.GetUserByClaimedEpisodeAsync(user.Id);

                await _claim.DeleteClaimByEpisodeIdAsync(claimedEpisode.Id);

                return Ok();
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Sequence contains no elements")) return BadRequest("Currently there are no claimed episodes by this user.");

                _logger.LogError($"Error trying to submit a claimed episode. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }

        [HttpPost("reassing")]
        public async Task<ActionResult<ClaimResponse>> ReassingEpisode([FromBody] TopicReasingPostRequest topicReasing)
        {
            User user = null;

            try
            {
                user = await _auth.GetUserFromValidToken(Request);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while trying to get user trying to reassing episode with id: {topicReasing.EpisodeId} to user with id: {topicReasing.UserId}. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
                return Problem();
            }

            try
            {
                if (!await _auth.CheckIfUserHasElivatedPermission(Request)) return Unauthorized();
                _logger.LogInformation($"A user with the id: {user.Id} and the role id: {user.RolesId} is reassigning the episode with the id: {topicReasing.EpisodeId} to the user with the id: {topicReasing.UserId}");

                EpisodeExtended episode = await _database.GetMinimalEpisodeAsync(topicReasing.EpisodeId);
                if (episode.Claimed) return BadRequest("Episode is already claimed!");

                ClaimResponse response = await _claim.ReassingClaimAsync(episode, topicReasing.UserId);

                return Ok(response);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while trying to reassing claim. Reassing was requested by user with id: {user.Id}. Episode requested to be reassigned is episode with id: {topicReasing.EpisodeId}. The user it should be reassigned to has is: {topicReasing.UserId}. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }

        [HttpPost("extend_time")]
        public async Task<ActionResult<ClaimResponse>> PostAdditionalTime()
        {
            try
            {
                User user = await _auth.GetUserFromValidToken(Request);
                Episode claimedEpisode = await _claim.GetUserByClaimedEpisodeAsync(user.Id);

                await _claim.AddExtraTimeToClaimAsync(user.Id);

                return Ok();
            }
            catch (TimeExtendedTooOftenException)
            {
                return BadRequest("The time was already extended too often.");
            }
            catch (ClaimNotNearEnoughToInvalidationException)
            {
                return BadRequest("Claim is not close enough to invalidation, please wait until the last 5 minutes to extend the valid until time.");
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Sequence contains no elements")) return BadRequest("Currently there are no claimed episodes by this user.");

                _logger.LogError($"Error extend claim time. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }
    }
}
