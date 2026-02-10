using Business.DTOs.Groups;
using Business.Services.Groups;
using Business.Services.Standings;
using Infra.ResultWrapper;
using Microsoft.AspNetCore.Mvc;
using Soccer.Filters;

namespace Soccer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController(IGroupService groupService) : ControllerBase
    {
        private readonly IGroupService groupService = groupService;

        [HttpPost]
        [TranslateResultToActionResult]
        public async Task<Result<CreateGroupResponse>> CreateGroup(CreateGroupRequest request)
        {
            var result = await groupService.CreateGroup(request);
            return result;
        }

        [HttpPatch]
        [TranslateResultToActionResult]
        public async Task<Result<UpdateGroupResponse>> UpdateGroup(UpdateGroupRequest request)
        {
            var result = await groupService.UpdateGroup(request);
            return result;
        }

        [HttpDelete("{id}")]
        [TranslateResultToActionResult]
        public async Task<Result<DeleteGroupResponse>> DeleteGroup(Guid id)
        {
            var result = await groupService.DeleteGroup(id);
            return result;
        }

        [HttpGet("{id}/standings")]
        [TranslateResultToActionResult]
        public async Task<Result<GroupStandingsResponse>> GetGroupStandings(Guid id, [FromServices] IStandingsService standingsService)
        {
            var result = await standingsService.GetGroupStandingsAsync(id);
            return result;
        }

        [HttpGet("tournament/{tournamentId}")]
        [TranslateResultToActionResult]
        public async Task<Result<TournamentGroupsResponseDto>> GetGroupsByTournament(Guid tournamentId)
        {
            var result = await groupService.GetGroupsByTournament(tournamentId);
            return result;
        }

        [HttpPost("assign-teams")]
        [TranslateResultToActionResult]
        public async Task<Result<AssignTeamsResponse>> AssignTeams(AssignTeamsRequest request)
        {
            var result = await groupService.AssignTeamsToGroup(request);
            return result;
        }
    }
}
