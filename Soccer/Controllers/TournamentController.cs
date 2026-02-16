using Business.DTOs.Tournaments;
using Business.Services.Standings;
using Business.Services.Tournaments;
using Infra.ResultWrapper;
using Microsoft.AspNetCore.Mvc;
using Soccer.Filters;

namespace Soccer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController(ITournamentService tournamentService, IStandingsService standingsService) : ControllerBase
    {
        private readonly ITournamentService tournamentService = tournamentService;
        private readonly IStandingsService _standingsService = standingsService;

        [HttpPost]
        [TranslateResultToActionResult]
        public async Task<Result<CreateTournamentResponse>> CreateTournament(CreateTournamentRequest request)
        {
            var result = await tournamentService.CreateTournament(request);
            return result;
        }

        [HttpPost("addTeamsToTournament")]
        [TranslateResultToActionResult]
        public async Task<Result<List<AddTeamToTournamentResponse>>> AddTeamsToTournament([FromBody] AddTeamsToTournamentRequest request)
        {
            return await tournamentService.AddTeamsToTournament(request);
        }

        [HttpPost("{id}/groups/draw")]
        [TranslateResultToActionResult]
        public async Task<Result<GenerateTournamentGroupsResponse>> GenerateGroups(Guid id)
        {
            var result = await tournamentService.GenerateGroupsAsync(id);
            return result;
        }

        [HttpPost("{id}/groups/regenerate")]
        [TranslateResultToActionResult]
        public async Task<Result<RegenerateGroupsResponse>> RegenerateGroups(Guid id)
        {
            var result = await tournamentService.RegenerateGroupsAsync(id);
            return result;
        }

        [HttpPost("{id}/matches/draw")]
        [TranslateResultToActionResult]
        public async Task<Result<GenerateTournamentMatchesResponse>> GenerateMatches(Guid id)
        {
            var result = await tournamentService.GenerateMatchesAsync(id);
            return result;
        }

        [HttpPost("{id}/matches/regenerate")]
        [TranslateResultToActionResult]
        public async Task<Result<GenerateTournamentMatchesResponse>> RegenerateMatches(Guid id)
        {
            var result = await tournamentService.RegenerateMatchesAsync(id);
            return result;
        }

        [HttpPost("{id}/reset-schedule")]
        [TranslateResultToActionResult]
        public async Task<Result<GenerateTournamentMatchesResponse>> ResetSchedule(Guid id)
        {
            var result = await tournamentService.ResetScheduleAsync(id);
            return result;
        }

        [HttpPatch]
        [TranslateResultToActionResult]
        public async Task<Result<UpdateTournamentResponse>> UpdateTournament(UpdateTournamentRequest request)
        {
            var result = await tournamentService.UpdateTournament(request);
            return result;
        }

        [HttpDelete("{id}")]
        [TranslateResultToActionResult]
        public async Task<Result<DeleteTournamentResponse>> DeleteTournament(Guid id)
        {
            var result = await tournamentService.DeleteTournament(id);
            return result;
        }

        [HttpGet("{id}/top-scorers")]
        [TranslateResultToActionResult]
        public async Task<Result<List<TopScorerDto>>> GetTopScorers(Guid id, [FromQuery] int? topN)
        {
            return await _standingsService.GetTournamentTopScorersAsync(id, topN);
        }

        /// <summary>Get overall tournament statistics (goals, assists, red/yellow cards, match count).</summary>
        [HttpGet("{id}/stats")]
        [TranslateResultToActionResult]
        public async Task<Result<TournamentStatsDto>> GetTournamentStats(Guid id)
        {
            return await _standingsService.GetTournamentStatsAsync(id);
        }

        /// <summary>Get per-player statistics for the tournament (goals, assists, cards, matches played).</summary>
        [HttpGet("{id}/player-stats")]
        [TranslateResultToActionResult]
        public async Task<Result<List<TournamentPlayerStatsDto>>> GetTournamentPlayerStats(Guid id)
        {
            return await _standingsService.GetTournamentPlayerStatsAsync(id);
        }

        [HttpGet("{id}")]
        [TranslateResultToActionResult]
        public async Task<Result<GetTournamentByIdResponse>> GetTournamentById(Guid id)
        {
            return await tournamentService.GetTournamentById(id);
        }

        [HttpGet]
        [TranslateResultToActionResult]
        public async Task<Result<GetAllTournamentsResponse>> GetAllTournaments()
        {
            return await tournamentService.GetAllTournaments();
        }

        [HttpGet("with-team-count")]
        [TranslateResultToActionResult]
        public async Task<Result<GetAllTournamentsWithTeamCountResponse>> GetAllTournamentsWithTeamCount()
        {
            return await tournamentService.GetAllTournamentsWithTeamCount();
        }
    }
}
