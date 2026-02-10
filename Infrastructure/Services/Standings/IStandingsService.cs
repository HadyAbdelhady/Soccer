using Business.DTOs.Tournaments;
using Business.DTOs.Groups;
using Infra.ResultWrapper;

namespace Business.Services.Standings
{
    public interface IStandingsService
    {
        Task<Result<GroupStandingsResponse>> GetGroupStandingsAsync(Guid groupId);
        Task<Result<List<TopScorerDto>>> GetTournamentTopScorersAsync(Guid tournamentId, int? topN);
    }
}
