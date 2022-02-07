using System.Collections.Generic;
using System.Threading.Tasks;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.Web.Services.Interfaces
{
    public interface IData
    {
        Task<EpisodeResponse> GetEpisodes(int count, int page);
        Task<EpisodeExtendedExtra> GetEpisode(int id);
        Task<LoginResponse> Login(string username, string password, bool keepLoggedIn);
        Task<List<TopicExtended>> GetTopics(int EpisodeID);
        Task ClaimEpisode(int episodeID);
        Task<List<TopicExtended>> PostTopic(List<TopicPostRequest> topic, List<PeopleInEpisode> people);
        Task<List<Themenschaedel.Shared.Models.Search>> Search(string searchTerm);
        Task<Episode> GetClaimedEpisode();
        Task<bool> IsCurrentlyClaimedEpisode(int episodeId);
    }
}
