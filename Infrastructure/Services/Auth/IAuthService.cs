using Business.DTOs.Teams;
using Infra.ResultWrapper;
using Infra.enums;

namespace Business.Services.Auth
{
    public interface IAuthService
    {
        Task<Result<LoginResponse>> Login(LoginRequest request);
        Task<Result<LoginResponse>> RegisterWatcherAsync(RegisterWatcherRequest request);
        Task<Result<SetFcmTokenResponse>> SetFcmTokenAsync(Guid userId, UserRole role, string? fcmToken);
        Task<Result<LogoutResponse>> LogoutAsync(Guid userId, UserRole role);
    }
}
