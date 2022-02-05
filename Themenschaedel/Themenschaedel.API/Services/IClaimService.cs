using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Services
{
    public interface IClaimService
    {
        public Task<ClaimResponse> ClaimEpisodeAsync(Episode episode, int userId);
        public void ClearExpiredClaimsAync();
        public Task<bool> HasUserClaimOnEpisodeAsync(int episodeId, int userId);
    }
}
