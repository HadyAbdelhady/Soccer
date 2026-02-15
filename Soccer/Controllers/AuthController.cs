using System.Security.Claims;
using Business.DTOs.Teams;
using Business.Services.Auth;
using Infra.ResultWrapper;
using Infra.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soccer.Filters;

namespace Soccer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        [TranslateResultToActionResult]
        public async Task<Result<LoginResponse>> Login(LoginRequest request)
        {
            return await authService.Login(request);
        }

        [HttpPost("register/watcher")]
        [TranslateResultToActionResult]
        public async Task<Result<LoginResponse>> RegisterWatcher([FromBody] RegisterWatcherRequest request)
        {
            return await authService.RegisterWatcherAsync(request);
        }

        [Authorize]
        [HttpPatch("me/fcm-token")]
        [TranslateResultToActionResult]
        public async Task<Result<SetFcmTokenResponse>> SetFcmToken([FromBody] SetFcmTokenRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Result<SetFcmTokenResponse>.FailureStatusCode("Invalid token", ErrorType.UnAuthorized);
            if (string.IsNullOrEmpty(roleClaim) || !Enum.TryParse<UserRole>(roleClaim, ignoreCase: true, out var role))
                return Result<SetFcmTokenResponse>.FailureStatusCode("Invalid token", ErrorType.UnAuthorized);

            return await authService.SetFcmTokenAsync(userId, role, request.FcmToken);
        }

        [Authorize]
        [HttpPost("logout")]
        [TranslateResultToActionResult]
        public async Task<Result<LogoutResponse>> Logout()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Result<LogoutResponse>.FailureStatusCode("Invalid token", ErrorType.UnAuthorized);
            if (string.IsNullOrEmpty(roleClaim) || !Enum.TryParse<UserRole>(roleClaim, ignoreCase: true, out var role))
                return Result<LogoutResponse>.FailureStatusCode("Invalid token", ErrorType.UnAuthorized);

            return await authService.LogoutAsync(userId, role);
        }
    }
}
