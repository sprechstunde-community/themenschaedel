using Microsoft.AspNetCore.Mvc;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;
using User = Themenschaedel.Shared.Models.User;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Themenschaedel.API.Controllers
{
    [Route("api/episodes")]
    [ApiController]
    public class EpisodesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<EpisodesController> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IClaimService _claims;
        private readonly ISearchService _search;
        public EpisodesController(IDatabaseService databaseService, ILogger<EpisodesController> logger, IAuthenticationService authenticationService, IClaimService claims, ISearchService searchService)
        {
            _databaseService = databaseService;
            _logger = logger;
            _authenticationService = authenticationService;
            _claims = claims;
            _search = searchService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<EpisodeExtendedExtra>> GetAllEpisodes()
        {
            try
            {
                List<EpisodeExtendedExtra> episodes = await _databaseService.GetAllEpisodesAsync();

                if (episodes.Count == 0)
                    return NoContent();

                return Ok(episodes);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError(e.Message);
                return Problem();
            }
        }

        [HttpGet("all/unverified")]
        public async Task<ActionResult<EpisodeAlternateResponse>> GetUnverifiedAllEpisodes(int page, int per_page)
        {
            if (page == 0) page = 1;

            try
            {
                User user = await _authenticationService.GetUserFromValidToken(Request);
                if (!await _authenticationService.CheckIfUserHasElivatedPermissionByUserObject(user)) return Unauthorized();

                EpisodeAlternateResponse episodeResponse = new EpisodeAlternateResponse();

                episodeResponse.Data = await _databaseService.GetEpisodeAwaitingVerificationAsync(page, per_page);
                episodeResponse.Meta.EpisodeCount = await _databaseService.GetUnverifiedEpisodeCountAsync();
                episodeResponse.Meta.EpisodeMaxPageCount = (int)Math.Ceiling((decimal)episodeResponse.Meta.EpisodeCount / per_page);

                if (episodeResponse.Data.Count == 0)
                    return NoContent();

                return Ok(episodeResponse);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError(e.Message);
                return Problem();
            }
        }

        // GET: api/<EpisodesController>
        [HttpGet]
        public async Task<ActionResult<EpisodeResponse>> GetEpisodes(int page, int per_page)
        {
            if (page == 0) page = 1;
            
            int userId = 0;
            try
            {
                User user = await _authenticationService.GetUserFromValidToken(Request);
                userId = user.Id;
            }
            catch (TokenDoesNotExistException e)
            {
                // ignore lol -> could happen all the time
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError(e.Message);
            }

            try
            {
                EpisodeResponse episodeResponse = new EpisodeResponse();

                episodeResponse.Data = await _databaseService.GetEpisodesAsync(page, per_page, userId);
                episodeResponse.Meta.EpisodeCount = await _databaseService.GetEpisodeCountAsync();
                episodeResponse.Meta.EpisodeMaxPageCount = (int)Math.Ceiling((decimal)episodeResponse.Meta.EpisodeCount / per_page);

                if (episodeResponse.Data.Count == 0)
                    return NoContent();

                return Ok(episodeResponse);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError(e.Message);
                return Problem();
            }
        }

        // GET: api/<EpisodesController>
        [HttpGet("{episodeId}")]
        public async Task<ActionResult> GetEpisode(int episodeId)
        {
            bool isEditingRequestedEpisode = false;
            int userId = 0;
            try
            {
                User user = await _authenticationService.GetUserFromValidToken(Request);
                userId = user.Id;
                if (await _authenticationService.CheckIfUserHasElivatedPermissionByUserObject(user))
                {
                    isEditingRequestedEpisode = true;
                }
                else
                {
                    Episode claimedEpisode = await _claims.GetUserByClaimedEpisodeAsync(user.Id);
                    isEditingRequestedEpisode = claimedEpisode.Id == episodeId;
                }
            }
            catch (TokenDoesNotExistException e)
            {
                // ignore lol -> could happen all the time
            }
            catch (InvalidOperationException e)
            {
                // Ignore
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError(e.Message);
            }

            try
            {
                return Ok(await _databaseService.GetEpisodeAsync(episodeId, isEditingRequestedEpisode, userId));
            }
            catch (InvalidOperationException e)
            {
                return NoContent();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError(e.Message);
                return Problem();
            }
        }

        // POST api/<EpisodesController>
        [HttpPost("verify/{id}")]
        public async Task<IActionResult> Post(int id)
        {
            try
            {
                User user = await _authenticationService.GetUserFromValidToken(Request);
                if (!await _authenticationService.CheckIfUserHasElivatedPermissionByUserObject(user)) return Unauthorized();

                EpisodeExtendedExtra episode = await _databaseService.GetEpisodeAsync(id, true, user.Id);
                if (episode.Topic == null || episode.Topic.Count == 0)
                {
                    return BadRequest("Episode has nothing to verify.");
                }

                if (episode.Verified) return BadRequest("Episode is already verified.");

                await _databaseService.VerifyEpisodeAsync(id);
                await _search.AddTopicsAsync(episode.Topic);
                return Ok();
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Sequence contains no elements")) return BadRequest("Episode is not open to verify.");

                SentrySdk.CaptureException(e);
                _logger.LogError(e.Message);
                return Problem();
            }
        }

        // DELETE api/<EpisodesController>/5
        [HttpDelete("unverify/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!await _authenticationService.CheckIfUserHasElivatedPermission(Request)) return Unauthorized();

                await _databaseService.UnverifyEpisodeAsync(id);
                return Ok();
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError(e.Message);
                return Problem();
            }
        }

        // DELETE api/<EpisodesController>/5
        [HttpDelete("delete/topics/{id}")]
        public async Task<IActionResult> DeleteAllTopicContent(int id)
        {
            try
            {
                if (!await _authenticationService.CheckIfUserHasElivatedPermission(Request)) return Unauthorized();

                await _databaseService.DeleteTopicAndSubtopicAsync(id);
                return Ok();
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError(e.Message);
                return Problem();
            }
        }
    }
}
