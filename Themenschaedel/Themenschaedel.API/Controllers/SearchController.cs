using Meilisearch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;

namespace Themenschaedel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _search;

        public SearchController(ISearchService searchService)
        {
            _search = searchService;
        }

        [HttpGet("episodes")]
        public async Task<ActionResult<SearchResult<Episode>>> GetEpisode([FromQuery(Name = "q")] string searchTerm)
        {
            if (String.IsNullOrWhiteSpace(searchTerm) || String.IsNullOrEmpty(searchTerm))
                return BadRequest("Search term cannot be empty!");

            return await _search.SearchEpisodesAsync(searchTerm);
        }

        [HttpGet("topics")]
        public async Task<ActionResult<SearchResult<Topic>>> GetTopics([FromQuery(Name = "q")] string searchTerm)
        {
            if (String.IsNullOrWhiteSpace(searchTerm) || String.IsNullOrEmpty(searchTerm))
                return BadRequest("Search term cannot be empty!");
            
            return await _search.SearchTopicsAsync(searchTerm);
        }

        [HttpGet("subtopics")]
        public async Task<ActionResult<SearchResult<Subtopic>>> GetSubtopics([FromQuery(Name = "q")] string searchTerm)
        {
            if (String.IsNullOrWhiteSpace(searchTerm) || String.IsNullOrEmpty(searchTerm))
                return BadRequest("Search term cannot be empty!");

            return await _search.SearchSubtopicsAsync(searchTerm);
        }
    }
}
