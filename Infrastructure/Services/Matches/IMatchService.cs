using Business.DTOs.Matches;
using Infra.ResultWrapper;

namespace Business.Services.Matches
{
    public interface IMatchService
    {
        Task<Result<MatchResponse>> CreateMatch(CreateMatchRequest request);
        Task<Result<SubmitResultResponse>> SubmitMatchResult(Guid matchId, SubmitResultRequest request);
        Task<Result<AddGoalResponse>> AddGoal(Guid matchId, GoalRequest request);
        Task<Result<AddCardResponse>> AddCard(Guid matchId, CardRequest request);
        Task<Result<UpdateMatchScheduleResponse>> UpdateMatchSchedule(Guid matchId, UpdateMatchScheduleRequest request);
        Task<Result<SetMatchLineupResponse>> SetMatchLineup(Guid matchId, SetMatchLineupRequest request);
        Task<Result<GetMatchLineupResponse>> GetMatchLineup(Guid matchId);
        Task<Result<GetMatchLineupResponse>> GetMatchLineupForTeam(Guid matchId, Guid teamId);
        Task<Result<GetAllMatchesResponse>> GetAllMatches(DateTime? date = null, Guid? teamId = null);
    }
}
