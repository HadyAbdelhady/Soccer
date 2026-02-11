using System.Security.Claims;

namespace Infra.Interface
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid userId, string email, string role, string fullName);
        ClaimsPrincipal? ValidateToken(string token);
        string GenerateRefreshToken();
    }
}
