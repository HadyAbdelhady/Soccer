using Business.DTOs.Matches;
using Infra.ResultWrapper;

namespace Business.Services.Matches
{
    public interface IMatchService
    {
        Task<Result<MatchResponse>> CreateMatch(CreateMatchRequest request);
        Task<Result<SubmitResultResponse>> SubmitMatchResult(Guid matchId, SubmitResultRequest request);
        Task<Result<UpdateMatchScheduleResponse>> UpdateMatchSchedule(Guid matchId, UpdateMatchScheduleRequest request);
    }
}
