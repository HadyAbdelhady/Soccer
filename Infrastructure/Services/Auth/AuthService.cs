using Business.DTOs.Teams;
using Data.Entities;
using Infra.Interface;
using Infra.ResultWrapper;
using Infra.enums;

namespace Business.Services.Auth
{
    public class AuthService(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher) : IAuthService
    {
        public async Task<Result<LoginResponse>> Login(LoginRequest request)
        {
            // 1) Try Admin/Viewer (User table)
            var user = await unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user != null)
            {
                if (!passwordHasher.VerifyPassword(request.Password, user.HashedPassword))
                    return Result<LoginResponse>.FailureStatusCode("Invalid username or password", ErrorType.UnAuthorized);

                var token = jwtTokenService.GenerateToken(user.Id, user.Username, user.Role, user.FullName);
                var refreshToken = jwtTokenService.GenerateRefreshToken();

                return Result<LoginResponse>.Success(new LoginResponse
                {
                    Id = user.Id,
                    Name = user.FullName,
                    Username = user.Username,
                    Role = user.Role,
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    Message = "Login successfully"
                });
            }

            // 2) Try Team
            var team = await unitOfWork.Repository<Team>()
                .FirstOrDefaultAsync(t => t.Username == request.Username);
            if (team != null)
            {
                var passwordValid = IsTeamPasswordValid(request.Password, team.HashedPassword);
                if (!passwordValid)
                    return Result<LoginResponse>.FailureStatusCode("Invalid username or password", ErrorType.UnAuthorized);

                var teamToken = jwtTokenService.GenerateToken(team.Id, team.Username, "Team", team.Name);
                var teamRefresh = jwtTokenService.GenerateRefreshToken();

                return Result<LoginResponse>.Success(new LoginResponse
                {
                    Id = team.Id,
                    Name = team.Name,
                    Username = team.Username,
                    Role = "Team",
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
