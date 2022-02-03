using Microsoft.AspNetCore.Mvc;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;

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
        public EpisodesController(IDatabaseService databaseService, ILogger<EpisodesController> logger, IAuthenticationService authenticationService)
        {
            _databaseService = databaseService;
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<EpisodeExtended>> GetAllEpisodes()
        {
            try
            {
                List<EpisodeExtended> episodes = await _databaseService.GetAllEpisodesAsync();

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
                EpisodeAlternateResponse episodeResponse = new EpisodeAlternateResponse();

                episodeResponse.Data = await _databaseService.GetEpisodeAwaitingVerificationAsync(page, per_page);
                episodeResponse.Meta.EpisodeCount = await _databaseService.GetUnverifiedEpisodeCount();
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
        [HttpGet]
        public async Task<ActionResult<EpisodeResponse>> GetEpisodes(int page, int per_page)
        {
            if (page == 0) page = 1;

            try
            {
                EpisodeResponse episodeResponse = new EpisodeResponse();

                episodeResponse.Data = await _databaseService.GetEpisodesAsync(page, per_page);
                episodeResponse.Meta.EpisodeCount = await _databaseService.GetEpisodeCount();
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
        public async Task<ActionResult<Episode>> GetEpisode(int episodeId)
        {
            try
            {
                Episode episode = await _databaseService.GetEpisodeAsync(episodeId);

                return Ok(episode);
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
        [HttpPost("verify_episode/{id}")]
        public async Task<IActionResult> Post(int id)
        {
            try
            {
                if (await _authenticationService.CheckIfUserHasElivatedPermission(Request)) return Unauthorized();

                EpisodeExtended episode = await _databaseService.GetEpisodeAsync(id);
                if (episode.Topic == new List<TopicExtended>())
                {
                    return BadRequest("Episode has nothin to verify.");
                }

                await _databaseService.VerifyEpisode(id);
                return Ok();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError(e.Message);
                return Problem();
            }
        }

        // DELETE api/<EpisodesController>/5
        [HttpDelete("unverify_epsidoe/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (await _authenticationService.CheckIfUserHasElivatedPermission(Request))
                {
                    await _databaseService.UnverifyEpisode(id);
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
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
