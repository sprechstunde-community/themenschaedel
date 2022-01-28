using Themenschaedel.Shared.Models;

namespace Themenschaedel.API.Services
{
    public interface IDatabaseService
    {
        public Task<List<Episode>> GetEpisodesAsync(int page, int per_page);
        public Task<List<Episode>> GetAllEpisodesAsync();
        public List<Episode> GetAllEpisodes();
        public void AddEpisodes(List<Episode> episodes);
    }
}
