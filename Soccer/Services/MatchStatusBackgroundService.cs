using Data.Entities;
using Infra.DBContext;
using Infra.enums;
using Microsoft.EntityFrameworkCore;

namespace Soccer.Services
{
    /// <summary>
    /// Automatically updates match status: SCHEDULED → LIVE at kickoff time, LIVE → FINISHED after match duration.
    /// </summary>
    public class MatchStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MatchStatusBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

        public MatchStatusBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<MatchStatusBackgroundService> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Match status background service started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateMatchStatusesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Match status update run failed.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }

        private async Task UpdateMatchStatusesAsync(CancellationToken cancellationToken)
        {
            var durationMinutes = _configuration.GetValue("Match:AutoFinishDurationMinutes", 105);
            var matchDuration = TimeSpan.FromMinutes(durationMinutes);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SoccerDbContext>();
            var nowUtc = DateTime.UtcNow;
            var matchEndThreshold = nowUtc.Subtract(matchDuration);

            // SCHEDULED → LIVE when KickoffTime has passed (and not already finished)
            var toLive = await db.Matches
                .Where(m => m.KickoffTime.HasValue
                    && m.Status == MatchStatus.SCHEDULED
                    && m.KickoffTime.Value <= nowUtc)
                .ToListAsync(cancellationToken);

            foreach (var m in toLive)
            {
                m.Status = MatchStatus.LIVE;
                m.UpdatedAt = DateTimeOffset.UtcNow;
            }

            // LIVE → FINISHED when KickoffTime + duration has passed
            var toFinished = await db.Matches
                .Where(m => m.KickoffTime.HasValue
                    && m.Status == MatchStatus.LIVE
                    && m.KickoffTime.Value.Add(matchDuration) <= nowUtc)
                .ToListAsync(cancellationToken);

            foreach (var m in toFinished)
            {
                m.Status = MatchStatus.FINISHED;
                m.FinalWhistleTime ??= m.KickoffTime!.Value.Add(matchDuration);
                m.UpdatedAt = DateTimeOffset.UtcNow;
            }

            var changed = toLive.Count + toFinished.Count;
            if (changed > 0)
            {
                await db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Match status updated: {Live} set to LIVE, {Finished} set to FINISHED.", toLive.Count, toFinished.Count);
            }
        }
    }
}
