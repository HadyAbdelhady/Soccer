using Business.DTOs.Tournaments;
using Business.Services.Tournaments;
using Infra.ResultWrapper;
using Microsoft.AspNetCore.Mvc;
using Soccer.Filters;

namespace Soccer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController(ITournamentService tournamentService) : ControllerBase
    {
        private readonly ITournamentService tournamentService = tournamentService;

        [HttpPost]
        [TranslateResultToActionResult]
        public async Task<Result<CreateTournamentResponse>> CreateTournament(CreateTournamentRequest request)
        {
            var result = await tournamentService.CreateTournament(request);
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
    }
}
