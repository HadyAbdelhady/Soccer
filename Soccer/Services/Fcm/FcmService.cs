using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace Soccer.Services.Fcm
{
    public class FcmService : IFcmService
    {
        private readonly ILogger<FcmService> _logger;
        private readonly string? _credentialsPath;
        private FirebaseApp? _app;

        public FcmService(IConfiguration configuration, ILogger<FcmService> logger)
        {
            _logger = logger;
            _credentialsPath = configuration["Firebase:CredentialsPath"];
        }

        private FirebaseApp GetOrCreateApp()
        {
            if (_app != null)
                return _app;

            if (string.IsNullOrWhiteSpace(_credentialsPath) || !File.Exists(_credentialsPath))
            {
                _logger.LogWarning("Firebase credentials not configured (Firebase:CredentialsPath) or file missing. FCM will be skipped.");
                return null!;
            }

            try
            {
                _app = FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(_credentialsPath)
                });
                _logger.LogInformation("Firebase app initialized for FCM.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase app.");
            }

            return _app!;
        }

        public async Task SendAsync(IReadOnlyList<string> tokens, string title, string body, CancellationToken cancellationToken = default)
        {
            var list = tokens?.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList() ?? new List<string>();
            if (list.Count == 0)
                return;

            var app = GetOrCreateApp();
            if (app == null)
                return;

            try
            {
                var messages = list.Select(token => new Message
                {
                    Token = token,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    }
                }).ToList();

                var response = await FirebaseMessaging.GetMessaging(app).SendAllAsync(messages, cancellationToken);
                _logger.LogInformation("FCM lineup reminder: sent {Success} of {Total} tokens.", response.SuccessCount, list.Count);
                if (response.FailureCount > 0)
                    _logger.LogWarning("FCM failures: {Count}", response.FailureCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FCM send failed.");
            }
        }
    }
}
