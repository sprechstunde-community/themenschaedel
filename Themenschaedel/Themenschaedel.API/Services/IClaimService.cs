using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Services
{
    public interface IClaimService
    {
        public Task<ClaimResponse> ClaimEpisodeAsync(int episodeId);
        public Task<List<Claim>> ClearExpiredClaimsAync();
    }
}
