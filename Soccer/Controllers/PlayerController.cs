using Business.DTOs.Players;
using Business.Services.Players;
using Infra.ResultWrapper;
using Microsoft.AspNetCore.Mvc;
using Soccer.Filters;

namespace Soccer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController(IPlayerService playerService) : ControllerBase
    {
        private readonly IPlayerService playerService = playerService;

        [HttpPost]
        [TranslateResultToActionResult]
        public async Task<Result<CreatePlayerResponse>> CreatePlayer(CreatePlayerRequest request)
        {
            var result = await playerService.CreatePlayer(request);
            return result;
        }

        [HttpPatch]
        [TranslateResultToActionResult]
        public async Task<Result<UpdatePlayerResponse>> UpdatePlayer(UpdatePlayerRequest request)
        {
            var result = await playerService.UpdatePlayer(request);
            return result;
        }

        [HttpDelete("{id}")]
        [TranslateResultToActionResult]
        public async Task<Result<DeletePlayerResponse>> DeletePlayer(Guid id)
        {
            var result = await playerService.DeletePlayer(id);
            return result;
        }

        [HttpGet("{id}")]
        [TranslateResultToActionResult]
        public async Task<Result<GetPlayerResponse>> GetPlayerById(Guid id)
        {
            var result = await playerService.GetPlayerById(id);
            return result;
        }

        [HttpGet]
        [TranslateResultToActionResult]
        public async Task<Result<IEnumerable<GetPlayerResponse>>> GetAllPlayers()
        {
            var result = await playerService.GetAllPlayers();
            return result;
        }
    }
}
