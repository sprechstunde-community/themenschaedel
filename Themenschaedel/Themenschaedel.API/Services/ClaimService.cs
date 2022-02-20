using Themenschaedel.Shared;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ILogger<ClaimService> _logger;
        private readonly IDatabaseService _database;

        private List<AddedClaimTime> AddedTime = new List<AddedClaimTime>();

        public ClaimService(ILogger<ClaimService> logger, IDatabaseService databaseService)
        {
            _logger = logger;
            _database = databaseService;
        }

        public async Task AddExtraTimeToClaimAsync(int userId)
        {
            Claim claim = await _database.GetClaimByUserIdAsync(userId);

            int index = AddedTime.FindIndex(x => x.UserId == userId && x.EpisodeId == claim.EpisodeId);
            if (index != -1)
            {
                if (AddedTime[index].Counter == 10) throw new TimeExtendedTooOftenException();
            }

            if ((claim.ValidUntil - DateTime.Now).TotalMinutes <= 5)
            {
                DateTime newValiDateTime = DateTime.Now.AddMinutes(30);
                await _database.UpdateClaimsValidUntilAsync(claim.Id, newValiDateTime);

                if (index == -1)
                {
                    AddedTime.Add(new AddedClaimTime()
                    {
                        UserId = userId,
                        EpisodeId = claim.EpisodeId,
                        Counter = 1
                    });
                }
                else
                {
                    AddedTime[index].Counter += 1;
                }
            }
            else
            {
                throw new ClaimNotNearEnoughToInvalidationException();
            }
        }

        public async Task<DateTime> GetValidUntilByUserId(int userId)
        {
            Claim claim = await _database.GetClaimByUserIdAsync(userId);
            return claim.ValidUntil;
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

        public async Task DeleteClaimByEpisodeIdAsync(int episodeId) =>
            await _database.DeleteClaimByEpisodeIdAsync(episodeId);

        public async Task<EpisodeWithValidUntilClaim> GetUserByClaimedEpisodeAsync(int userId) =>
            await _database.GetClaimedEpisodeByUserIdAsync(userId);

        public async Task<bool> HasUserClaimOnEpisodeAsync(int episodeId, int userId) =>
            await _database.CheckIfUserHasClaimOnEpisodeAsync(episodeId, userId);

        public async Task<ClaimResponse> ReassingClaimAsync(Episode episode, int userId)
        {
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
    }

    public class AddedClaimTime
    {
        public int EpisodeId { get; set; }
        public int UserId { get; set; }
        public int Counter { get; set; }
    }
}
