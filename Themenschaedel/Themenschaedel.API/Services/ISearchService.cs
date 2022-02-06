using Meilisearch;
using Themenschaedel.Shared.Models;

namespace Themenschaedel.API.Services
{
    public interface ISearchService
    {
        // Add to search Database
        public Task AddEpisodeAsync(Episode episode);
        public Task AddEpisodesAsync(List<Episode> episodes);
        public void AddEpisodes(List<Episode> episodes);
        public Task AddTopicsAsync(List<TopicExtended> topic);
        public Task AddSubtopicsAsync(List<Subtopic> subtopics);

        // Search
        public Task<SearchResult<Episode>> SearchEpisodesAsync(string searchTerm);
        public Task<SearchResult<Topic>> SearchTopicsAsync(string searchTerm);
        public Task<SearchResult<Subtopic>> SearchSubtopicsAsync(string searchTerm);
    }
}
