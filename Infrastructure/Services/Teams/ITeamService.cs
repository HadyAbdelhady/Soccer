using Business.DTOs.Teams;
using Infra.ResultWrapper;

namespace Business.Services.Teams
{
    public interface ITeamService
    {
        Task<Result<CreateTeamResponse>> CreateTeam(CreateTeamRequest request);
        Task<Result<UpdateTeamResponse>> UpdateTeam(UpdateTeamRequest request);
        Task<Result<DeleteTeamResponse>> DeleteTeam(Guid id);
        Task<Result<LoginResponse>> Login(LoginRequest request);
    }
}
