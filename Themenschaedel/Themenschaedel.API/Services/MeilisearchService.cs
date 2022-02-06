using Meilisearch;
using Themenschaedel.Shared.Models;
using Index = Meilisearch.Index;

namespace Themenschaedel.API.Services
{
    public class MeilisearchService : ISearchService
    {
        private readonly IConfiguration _config;
        private MeilisearchClient _client;

        private Index _episodeIndex;
        private Index _topicIndex;
        private Index _subtopicIndex;

        public MeilisearchService(IConfiguration config)
        {
            _config = config;
            _client = new MeilisearchClient(_config["Meilisearch:URL"], _config["Meilisearch:Masterkey"]);
            _episodeIndex = _client.Index("episodes");
            _topicIndex = _client.Index("topics");
            _subtopicIndex = _client.Index("subtopics");
        }

        public async Task AddEpisodeAsync(Episode episode)
        {
            List<Episode> episodeList = new List<Episode>();
            episodeList.Add(episode);
            await _episodeIndex.AddDocumentsAsync<Episode>(episodeList);
        }

        public void AddEpisodes(List<Episode> episodes)
        {
            _episodeIndex.AddDocumentsAsync<Episode>(episodes).RunSynchronously();
        }

        public async Task AddEpisodesAsync(List<Episode> episodes)
        {
            await _episodeIndex.AddDocumentsAsync<Episode>(episodes);
        }

        public async Task AddSubtopicsAsync(List<Subtopic> subtopics)
        {
            await _subtopicIndex.AddDocumentsAsync<Subtopic>(subtopics);
        }

        public async Task AddTopicsAsync(List<TopicExtended> topic)
        {
            foreach (TopicExtended item in topic)
            {
                await AddSubtopicsAsync(item.Subtopic);
            }
            await _topicIndex.AddDocumentsAsync<Topic>(topic);
        }

        public async Task<SearchResult<Episode>> SearchEpisodesAsync(string searchTerm)
        {
            return await _episodeIndex.SearchAsync<Episode>(searchTerm);
        }

        public async Task<SearchResult<Subtopic>> SearchSubtopicsAsync(string searchTerm)
        {
            return await _subtopicIndex.SearchAsync<Subtopic>(searchTerm);
        }

        public async Task<SearchResult<Topic>> SearchTopicsAsync(string searchTerm)
        {
            return await _topicIndex.SearchAsync<Topic>(searchTerm);
        }
    }
}
