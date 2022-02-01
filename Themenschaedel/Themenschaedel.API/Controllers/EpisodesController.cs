using Microsoft.AspNetCore.Mvc;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Themenschaedel.API.Controllers
{
    [Route("api/episodes")]
    [ApiController]
    public class EpisodesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        public EpisodesController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
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
                return StatusCode(500, e.Message);
            }
        }

        // GET: api/<EpisodesController>
        [HttpGet]
        public async Task<ActionResult<Episode>> GetEpisodes(int page, int per_page)
        {
            if (page == 0) page = 1;

            try
            {
                List<Episode> episodes = await _databaseService.GetEpisodesAsync(page, per_page);

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
                return StatusCode(500, e.Message);
            }
        }

        // POST api/<EpisodesController>
        [HttpPost("verify_episode/{id}")]
        public void Post(int id)
        {
        }

        // DELETE api/<EpisodesController>/5
        [HttpDelete("unverify_epsidoe/{id}")]
        public void Delete(int id)
        {
        }
    }
}
