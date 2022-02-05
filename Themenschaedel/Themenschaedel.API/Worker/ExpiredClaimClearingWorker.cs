using FluentScheduler;
using Themenschaedel.API.Services;

namespace Themenschaedel.API.Worker
{
    public class ExpiredClaimClearingWorker : BackgroundService
    {
        private readonly ILogger<RssFeedScrapperWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IClaimService _claims;

        public ExpiredClaimClearingWorker(ILogger<RssFeedScrapperWorker> logger, IConfiguration configuration, IClaimService claimService)
        {
            _logger = logger;
            _configuration = configuration;
            _claims = claimService;
            JobManager.Initialize();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            JobManager.AddJob(
                () => this.ClearAllExpiredClaims(),
                s => s.ToRunNow().AndEvery(1).Minutes()
            );
        }

        protected void ClearAllExpiredClaims()
        {
            _logger.LogDebug("Starting automated token clearing process.");
            _claims.ClearExpiredClaimsAync();
            _logger.LogDebug("Finished automated token clearing process.");
        }
    }
}
