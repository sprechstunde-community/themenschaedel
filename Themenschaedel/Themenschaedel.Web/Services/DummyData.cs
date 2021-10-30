using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Themenschaedel.Shared.Props;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Web.Services
{
    public class DummyData : IData
    {
        public DummyData(HttpClient httpClient)
        {

        }
        public async Task<Episode> GetEpisode(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Token> Login(string username, string password, bool keepLoggedIn = false)
        {
            throw new NotImplementedException();
        }

        public Task<GetTopicWorkaroundWrapper> GetTopics(int EpisodeID)
        {
            throw new NotImplementedException();
        }

        public Task ClaimEpisode(int episodeID)
        {
            throw new NotImplementedException();
        }

        public Task<Topic> AddTopic(Topic topic, int episodeID)
        {
            throw new NotImplementedException();
        }

        public Task<Topic> UpdateTopic(Topic topic)
        {
            throw new NotImplementedException();
        }

        public Task<Topic> UpdateTopic(Topic topic, int episodeID)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTopic(Topic topic)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSubtopic(Subtopics subtopic)
        {
            throw new NotImplementedException();
        }

        public Task<Topic> AddTopic()
        {
            throw new NotImplementedException();
        }

        public async Task<GetEpisodesWorkaround> GetEpisodes(int count, int page)
        {
            GetEpisodesWorkaround ep = new GetEpisodesWorkaround();
            ep.data = new List<Episode>();
            ep.meta = new WorkaroundMeta();
            ep.meta.current_page = page;
            ep.meta.last_page = 3;
            for (int i = 0; i < count; i++)
            {
                ep.data.Add(new Episode()
                {
                    title = "test",
                    image = null,
                    id = i,
                    duration = 5234 + i,
                    episode_number = i,
                });
            }
            await Task.Delay(1000);
            return ep;
        }
    }
}
