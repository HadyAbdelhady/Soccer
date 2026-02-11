using Business.DTOs.Teams;
using Business.Services.Teams;
using Infra.ResultWrapper;
using Microsoft.AspNetCore.Mvc;
using Soccer.Filters;

namespace Soccer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController(ITeamService teamService) : ControllerBase
    {
        private readonly ITeamService teamService = teamService;

        [HttpPost]
        [TranslateResultToActionResult]
        public async Task<Result<CreateTeamResponse>> CreateTeam(CreateTeamRequest request)
        {
            var result = await teamService.CreateTeam(request);
            return result;
        }

        [HttpPatch]
        [TranslateResultToActionResult]
        public async Task<Result<UpdateTeamResponse>> UpdateTeam(UpdateTeamRequest request)
        {
            var result = await teamService.UpdateTeam(request);
            return result;
        }

        [HttpDelete("{id}")]
        [TranslateResultToActionResult]
        public async Task<Result<DeleteTeamResponse>> DeleteTeam(Guid id)
        {
            var result = await teamService.DeleteTeam(id);
            return result;
        }
    }
}
