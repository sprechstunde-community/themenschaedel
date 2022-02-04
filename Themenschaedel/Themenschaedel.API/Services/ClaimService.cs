using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ILogger<ClaimService> _logger;
        private readonly IDatabaseService _database;
        public ClaimService(ILogger<ClaimService> logger, IDatabaseService databaseService)
        {
            _logger = logger;
            _database = databaseService;
        }

        public async Task<ClaimResponse> ClaimEpisodeAsync(int episodeId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Claim>> ClearExpiredClaimsAync()
        {
            throw new NotImplementedException();
        }
    }
}
