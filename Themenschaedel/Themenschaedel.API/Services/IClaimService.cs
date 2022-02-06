using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Services
{
    public interface IClaimService
    {
        public Task<ClaimResponse> ClaimEpisodeAsync(Episode episode, int userId);
        public Task<ClaimResponse> ReassingClaimAsync(Episode episode, int userId);
        public void ClearExpiredClaimsAync();
        public Task<bool> HasUserClaimOnEpisodeAsync(int episodeId, int userId);
        public Task<Episode> GetUserByClaimedEpisodeAsync(int userId);
        public Task AddExtraTimeToClaimAsync(int userId);
        public Task DeleteClaimByEpisodeIdAsync(int episodeId);
    }
}
