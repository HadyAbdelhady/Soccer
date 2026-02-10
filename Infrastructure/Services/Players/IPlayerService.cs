using Business.DTOs.Players;
using Infra.ResultWrapper;

namespace Business.Services.Players
{
    public interface IPlayerService
    {
        Task<Result<CreatePlayerResponse>> CreatePlayer(CreatePlayerRequest request);
        Task<Result<UpdatePlayerResponse>> UpdatePlayer(UpdatePlayerRequest request);
        Task<Result<DeletePlayerResponse>> DeletePlayer(Guid id);
        Task<Result<GetPlayerResponse>> GetPlayerById(Guid id);
        Task<Result<IEnumerable<GetPlayerResponse>>> GetAllPlayers();
    }
}
