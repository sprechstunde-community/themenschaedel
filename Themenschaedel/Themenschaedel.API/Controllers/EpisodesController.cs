using Microsoft.AspNetCore.Mvc;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Themenschaedel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        public EpisodesController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/<EpisodesController>
        [HttpGet]
        public async Task<IActionResult> GetEpisodes(int page, int per_page)
        {
            if (page > 0) page -= 1;

            try
            {
                List<Episode> episodes = await _databaseService.GetEpisodesAsync(page, per_page);

                if (episodes.Count == 0)
                    return NotFound();

                return Ok(episodes);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return StatusCode(500, e.Message);
            }
        }

        // GET api/<EpisodesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EpisodesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<EpisodesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EpisodesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
