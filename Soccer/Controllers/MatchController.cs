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
    }
}
