using Business.DTOs.Teams;
using Infra.ResultWrapper;

namespace Business.Services.Auth
{
    public interface IAuthService
    {
        Task<Result<LoginResponse>> Login(LoginRequest request);
    }
}
