using Themenschaedel.Shared;
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

        public async Task<ClaimResponse> ClaimEpisodeAsync(Episode episode, int userId)
        {
            List<TopicExtended> topics = await _database.GetTopicsAsync(episode.Id);
            if (topics == null) throw new EpisodeUnclaimedButAlreadyHasTopcisException();
            if (topics.Count != 0) throw new EpisodeUnclaimedButAlreadyHasTopcisException();

            DateTime currentTime = DateTime.Now;
            DateTime claimValidUntil = currentTime.AddSeconds(episode.Duration * 3);

            await _database.ClaimEpisodeAsync(episode.Id, userId, claimValidUntil, currentTime);

            return new ClaimResponse()
            {
                EpisodeId = episode.Id,
                UserId = userId,
                ValidUntil = claimValidUntil
            };
        }

        public void ClearExpiredClaimsAync()
        {
            List<Claim> claims = _database.GetAllExpiredClaims();
            if (claims.Count == 0) return;
            _logger.LogError($"Clearing all these expired claims:\n{ObjectLogger.Dump(claims)}");
            _database.ClearAllExpiredClaims();
        }

        public async Task<Episode> GetUserByClaimedEpisodeAsync(int userId) =>
            await _database.GetClaimedEpisodeByUserIdAsync(userId);

        public async Task<bool> HasUserClaimOnEpisodeAsync(int episodeId, int userId) =>
            await _database.CheckIfUserHasClaimOnEpisodeAsync(episodeId, userId);
    }
}
