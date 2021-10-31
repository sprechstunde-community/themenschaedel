using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Themenschaedel.Shared.Props;

namespace Themenschaedel.Web.Services.Interfaces
{
    public interface IData
    {
        Task<GetEpisodesWorkaround> GetEpisodes(int count, int page);
        Task<Episode> GetEpisode(int id);
        Task<Token> Login(string username, string password, bool keepLoggedIn);
        Task<GetTopicWorkaroundWrapper> GetTopics(int EpisodeID);
        Task ClaimEpisode(int episodeID);
        Task<Topic> AddTopic(Topic topic, int episodeID);
        Task<Topic> UpdateTopic(Topic topic);
        Task DeleteTopic(Topic topic);
        Task DeleteSubtopic(Subtopics subtopic);
        Task<List<Search>> Search(string searchTerm);
    }
}
