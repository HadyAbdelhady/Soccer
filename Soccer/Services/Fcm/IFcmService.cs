namespace Soccer.Services.Fcm
{
    /// <summary>
    /// Sends FCM push notifications to device tokens.
    /// </summary>
    public interface IFcmService
    {
        /// <summary>
        /// Sends a notification to the given device tokens. Skips null/empty tokens.
        /// If Firebase is not configured, does nothing.
        /// </summary>
        Task SendAsync(IReadOnlyList<string> tokens, string title, string body, CancellationToken cancellationToken = default);
    }
}
