using Business.DTOs.Matches;
using Business.Services.Matches;
using Infra.ResultWrapper;
using Microsoft.AspNetCore.Mvc;
using Soccer.Filters;

namespace Soccer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController(IMatchService matchService) : ControllerBase
    {
        private readonly IMatchService matchService = matchService;

        [HttpPost]
        [TranslateResultToActionResult]
        public async Task<Result<MatchResponse>> CreateMatch(CreateMatchRequest request)
        {
            var result = await matchService.CreateMatch(request);
            return result;
        }

        [HttpPost("{id}/result")]
        [TranslateResultToActionResult]
        public async Task<Result<SubmitResultResponse>> SubmitResult(Guid id, SubmitResultRequest request)
        {
            var result = await matchService.SubmitMatchResult(id, request);
            return result;
        }

        [HttpPatch("{id}/schedule")]
        [TranslateResultToActionResult]
        public async Task<Result<UpdateMatchScheduleResponse>> UpdateSchedule(Guid id, UpdateMatchScheduleRequest request)
        {
            var result = await matchService.UpdateMatchSchedule(id, request);
            return result;
        }

        [HttpPost("{id}/lineup")]
        [TranslateResultToActionResult]
        public async Task<Result<SetMatchLineupResponse>> SetLineup(Guid id, SetMatchLineupRequest request)
        {
            var result = await matchService.SetMatchLineup(id, request);
            return result;
        }

        [HttpGet("{id}/lineup")]
        [TranslateResultToActionResult]
        public async Task<Result<GetMatchLineupResponse>> GetLineup(Guid id)
        {
            var result = await matchService.GetMatchLineup(id);
            return result;
        }

        [HttpGet("{id}/lineup/{teamId}")]
        [TranslateResultToActionResult]
        public async Task<Result<GetMatchLineupResponse>> GetLineupForTeam(Guid id, Guid teamId)
        {
            var result = await matchService.GetMatchLineupForTeam(id, teamId);
            return result;
        }

        /// <summary>Get all matches across all tournaments. Optionally filter by date and/or team (matches where team is home or away).</summary>
        [HttpGet("getAllMatches")]
        [TranslateResultToActionResult]
        public async Task<Result<GetAllMatchesResponse>> GetAllMatches([FromQuery] DateTime? date = null, [FromQuery] Guid? teamId = null)
        {
            return await matchService.GetAllMatches(date, teamId);
        }
    }
}
