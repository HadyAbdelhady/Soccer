using Business.DTOs.Tournaments;
using Infra.ResultWrapper;

namespace Business.Services.Tournaments
{
    public interface ITournamentService
    {
        Task<Result<CreateTournamentResponse>> CreateTournament(CreateTournamentRequest request);
        Task<Result<UpdateTournamentResponse>> UpdateTournament(UpdateTournamentRequest request);
        Task<Result<DeleteTournamentResponse>> DeleteTournament(Guid id);
        Task<Result<GenerateTournamentGroupsResponse>> GenerateGroupsAsync(Guid tournamentId);
        Task<Result<GenerateTournamentMatchesResponse>> GenerateMatchesAsync(Guid tournamentId);
        Task<Result<bool>> ResolvePlaceholders(Guid tournamentId);
    }
}