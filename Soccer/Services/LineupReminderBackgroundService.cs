using Data.Entities;
using Infra.DBContext;
using Microsoft.EntityFrameworkCore;
using Soccer.Services.Fcm;

namespace Soccer.Services
{
    /// <summary>
    /// Sends FCM push notifications to match teams one hour before kickoff, reminding them to submit their lineup.
    /// Ensures each match is reminded only once (tracked by LineupReminderSentAt).
    /// </summary>
    public class LineupReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LineupReminderBackgroundService> _logger;
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan OneHourMin = TimeSpan.FromMinutes(55);
        private static readonly TimeSpan OneHourMax = TimeSpan.FromMinutes(65);

        public LineupReminderBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<LineupReminderBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Lineup reminder background service started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendDueRemindersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lineup reminder run failed.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }

        private async Task SendDueRemindersAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SoccerDbContext>();
            var fcm = scope.ServiceProvider.GetRequiredService<IFcmService>();

            var nowUtc = DateTime.UtcNow;
            var minKickoff = nowUtc.Add(OneHourMin);
            var maxKickoff = nowUtc.Add(OneHourMax);

            var matches = await db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.KickoffTime.HasValue
                    && m.LineupReminderSentAt == null
                    && m.Status == Infra.enums.MatchStatus.SCHEDULED)
                .Where(m => m.KickoffTime!.Value >= minKickoff && m.KickoffTime.Value <= maxKickoff)
                .ToListAsync(cancellationToken);

            if (matches.Count == 0)
                return;

            var sentAt = DateTimeOffset.UtcNow;
            foreach (var match in matches)
            {
                var tokens = new List<string>();
                if (match.HomeTeam?.FcmToken != null)
                    tokens.Add(match.HomeTeam.FcmToken);
                if (match.AwayTeam?.FcmToken != null && match.AwayTeam.Id != match.HomeTeam?.Id)
                    tokens.Add(match.AwayTeam.FcmToken);

                if (tokens.Count > 0)
                {
                    await fcm.SendAsync(
                        tokens,
                        "Lineup reminder",
                        "Your match starts in about 1 hour. Please submit your lineup.",
                        cancellationToken);
                }

                match.LineupReminderSentAt = sentAt;
            }

            await db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Lineup reminders sent for {Count} match(es).", matches.Count);
        }
    }
}
