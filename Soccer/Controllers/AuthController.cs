using Business.DTOs.Teams;
using Business.Services.Auth;
using Infra.ResultWrapper;
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
    }
}
