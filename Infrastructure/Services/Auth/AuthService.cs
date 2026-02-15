using Infra.ResultWrapper;
using Business.DTOs.Teams;
using Data.Entities;
using Infra.Interface;
using Infra.enums;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.Auth
{
    public class AuthService(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher) : IAuthService
    {
        public async Task<Result<LoginResponse>> Login(LoginRequest request)
        {
            // 1) Try AdminUser (Admin)
            var adminUser = await unitOfWork.Repository<AdminUser>()
                .FirstOrDefaultAsync(u => u.Username == request.Username);
            if (adminUser != null)
            {
                if (!passwordHasher.VerifyPassword(request.Password, adminUser.HashedPassword))
                    return Result<LoginResponse>.FailureStatusCode("Invalid username or password", ErrorType.UnAuthorized);

                var token = jwtTokenService.GenerateToken(adminUser.Id, adminUser.Username, UserRole.Admin, adminUser.FullName);
                var refreshToken = jwtTokenService.GenerateRefreshToken();

                return Result<LoginResponse>.Success(new LoginResponse
                {
                    Id = adminUser.Id,
                    Name = adminUser.FullName,
                    Username = adminUser.Username,
                    Role = UserRole.Admin,
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    Message = "Login successfully"
                });
            }

            // 2) Try WatcherUser (Watcher)
            var watcherUser = await unitOfWork.Repository<WatcherUser>()
                .FirstOrDefaultAsync(u => u.Username == request.Username);
            if (watcherUser != null)
            {
                if (!passwordHasher.VerifyPassword(request.Password, watcherUser.HashedPassword))
                    return Result<LoginResponse>.FailureStatusCode("Invalid username or password", ErrorType.UnAuthorized);

                var token = jwtTokenService.GenerateToken(watcherUser.Id, watcherUser.Username, UserRole.Viewer, watcherUser.FullName);
                var refreshToken = jwtTokenService.GenerateRefreshToken();

                return Result<LoginResponse>.Success(new LoginResponse
                {
                    Id = watcherUser.Id,
                    Name = watcherUser.FullName,
                    Username = watcherUser.Username,
                    Role = UserRole.Viewer,
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    Message = "Login successfully"
                });
            }

            // 3) Try TeamUser (Team)
            var teamUser = await unitOfWork.Repository<TeamUser>()
                .FirstOrDefaultAsync(t => t.Username == request.Username);
            if (teamUser != null)
            {
                var passwordValid = IsTeamPasswordValid(request.Password, teamUser.HashedPassword);
                if (!passwordValid)
                    return Result<LoginResponse>.FailureStatusCode("Invalid username or password", ErrorType.UnAuthorized);

                var teamToken = jwtTokenService.GenerateToken(teamUser.Id, teamUser.Username, UserRole.Team, teamUser.FullName);
                var teamRefresh = jwtTokenService.GenerateRefreshToken();

                return Result<LoginResponse>.Success(new LoginResponse
                {
                    Id = teamUser.Id,
                    Name = teamUser.FullName,
                    Username = teamUser.Username,
                    Role = UserRole.Team,
                    AccessToken = teamToken,
                    RefreshToken = teamRefresh,
                    Message = "Login successfully"
                });
            }

            return Result<LoginResponse>.FailureStatusCode("Invalid username or password", ErrorType.UnAuthorized);
        }

        public async Task<Result<LoginResponse>> RegisterWatcherAsync(RegisterWatcherRequest request)
        {
            var usernameTaken = await unitOfWork.Repository<BaseUser>()
                .AnyAsync(u => u.Username == request.Username.Trim());
            if (usernameTaken)
                return Result<LoginResponse>.FailureStatusCode("Username is already taken.", ErrorType.BadRequest);

            var watcher = new WatcherUser
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName.Trim(),
                Username = request.Username.Trim(),
                HashedPassword = passwordHasher.HashPassword(request.Password),
                IsDeleted = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await unitOfWork.Repository<WatcherUser>().AddAsync(watcher);
            await unitOfWork.SaveChangesAsync();

            var token = jwtTokenService.GenerateToken(watcher.Id, watcher.Username, UserRole.Viewer, watcher.FullName);
            var refreshToken = jwtTokenService.GenerateRefreshToken();

            return Result<LoginResponse>.Success(new LoginResponse
            {
                Id = watcher.Id,
                Name = watcher.FullName,
                Username = watcher.Username,
                Role = UserRole.Viewer,
                AccessToken = token,
                RefreshToken = refreshToken,
                Message = "Registered successfully"
            });
        }

        public async Task<Result<SetFcmTokenResponse>> SetFcmTokenAsync(Guid userId, UserRole role, string? fcmToken)
        {
            if (role == UserRole.Admin)
            {
                var admin = await unitOfWork.Repository<AdminUser>().GetByIdAsync(userId);
                if (admin == null)
                    return Result<SetFcmTokenResponse>.FailureStatusCode("User not found", ErrorType.NotFound);
                admin.FcmToken = fcmToken;
                admin.UpdatedAt = DateTimeOffset.UtcNow;
                unitOfWork.Repository<AdminUser>().Update(admin);
            }
            else if (role == UserRole.Viewer)
            {
                var watcher = await unitOfWork.Repository<WatcherUser>().GetByIdAsync(userId);
                if (watcher == null)
                    return Result<SetFcmTokenResponse>.FailureStatusCode("User not found", ErrorType.NotFound);
                watcher.FcmToken = fcmToken;
                watcher.UpdatedAt = DateTimeOffset.UtcNow;
                unitOfWork.Repository<WatcherUser>().Update(watcher);
            }
            else if (role == UserRole.Team)
            {
                var team = await unitOfWork.Repository<TeamUser>().GetByIdAsync(userId);
                if (team == null)
                    return Result<SetFcmTokenResponse>.FailureStatusCode("User not found", ErrorType.NotFound);
                team.FcmToken = fcmToken;
                team.UpdatedAt = DateTimeOffset.UtcNow;
                unitOfWork.Repository<TeamUser>().Update(team);
            }
            else
            {
                return Result<SetFcmTokenResponse>.FailureStatusCode("Unknown role", ErrorType.BadRequest);
            }

            await unitOfWork.SaveChangesAsync();
            return Result<SetFcmTokenResponse>.Success(new SetFcmTokenResponse());
        }

        public async Task<Result<LogoutResponse>> LogoutAsync(Guid userId, UserRole role)
        {
            // Clear FCM token so the user stops receiving push notifications until they log in again.
            await SetFcmTokenAsync(userId, role, null);
            return Result<LogoutResponse>.Success(new LogoutResponse());
        }

        /// <summary>
        /// Validates password: BCrypt hash or legacy plain-text (for backward compatibility).
        /// </summary>
        private bool IsTeamPasswordValid(string password, string hashedPassword)
        {
            if (hashedPassword.StartsWith("$2", StringComparison.Ordinal))
                return passwordHasher.VerifyPassword(password, hashedPassword);
            return password == hashedPassword;
        }
    }
}
