using Business.DTOs.Groups;
using Infra.ResultWrapper;

namespace Business.Services.Groups
{
    public interface IGroupService
    {
        Task<Result<CreateGroupResponse>> CreateGroup(CreateGroupRequest request);
        Task<Result<UpdateGroupResponse>> UpdateGroup(UpdateGroupRequest request);
        Task<Result<DeleteGroupResponse>> DeleteGroup(Guid id);
        Task<Result<TournamentGroupsResponseDto>> GetGroupsByTournament(Guid tournamentId);
        Task<Result<AssignTeamsResponse>> AssignTeamsToGroup(AssignTeamsRequest request);
    }
}
