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
