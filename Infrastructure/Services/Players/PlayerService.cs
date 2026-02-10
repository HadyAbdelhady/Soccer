using Business.DTOs.Players;
using Data.Entities;
using Infra.enums;
using Infra.Interface;
using Infra.ResultWrapper;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.Players
{
    public class PlayerService(IUnitOfWork unitOfWork) : IPlayerService
    {

        public async Task<Result<CreatePlayerResponse>> CreatePlayer(CreatePlayerRequest request)
        {
            var team = await unitOfWork.Repository<Team>().GetByIdAsync(request.TeamId);
            if (team == null)
            {
                return Result<CreatePlayerResponse>.FailureStatusCode("Team not found", ErrorType.NotFound);
            }

            var player = new Player
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                NickName = request.NickName,
                Position = request.Position,
                JerseyNumber = request.JerseyNumber,
                IsCaptain = request.IsCaptain,
                TeamId = request.TeamId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await unitOfWork.Repository<Player>().AddAsync(player);
            await unitOfWork.SaveChangesAsync();

            var response = new CreatePlayerResponse
            {
                Id = player.Id,
                FullName = player.FullName,
                Message = "Created Successfully"
            };

            return Result<CreatePlayerResponse>.Success(response);
        }

        public async Task<Result<UpdatePlayerResponse>> UpdatePlayer(UpdatePlayerRequest request)
        {
            var player = await unitOfWork.Repository<Player>().GetByIdAsync(request.Id);
            if (player == null)
            {
                return Result<UpdatePlayerResponse>.FailureStatusCode("Player not found", ErrorType.NotFound);
            }

            var team = await unitOfWork.Repository<Team>().GetByIdAsync(request.TeamId);
            if (team == null)
            {
                return Result<UpdatePlayerResponse>.FailureStatusCode("Team not found", ErrorType.NotFound);
            }

            player.FullName = request.FullName;
            player.NickName = request.NickName;
            player.Position = request.Position;
            player.JerseyNumber = request.JerseyNumber;
            player.IsCaptain = request.IsCaptain;
            player.TeamId = request.TeamId;
            player.UpdatedAt = DateTimeOffset.UtcNow;

            unitOfWork.Repository<Player>().Update(player);
            await unitOfWork.SaveChangesAsync();

            var response = new UpdatePlayerResponse
            {
                Id = player.Id,
                FullName = player.FullName,
                Message = "Updated Successfully"
            };

            return Result<UpdatePlayerResponse>.Success(response);
        }

        public async Task<Result<DeletePlayerResponse>> DeletePlayer(Guid id)
        {
            var player = await unitOfWork.Repository<Player>().GetByIdAsync(id);
            if (player == null)
            {
                return Result<DeletePlayerResponse>.FailureStatusCode("Player not found", ErrorType.NotFound);
            }

            player.UpdatedAt = DateTimeOffset.UtcNow;
            unitOfWork.Repository<Player>().Remove(player);
            await unitOfWork.SaveChangesAsync();

            var response = new DeletePlayerResponse
            {
                Id = id,
                IsDeleted = true,
                Message = "Deleted Successfully"
            };

            return Result<DeletePlayerResponse>.Success(response);
        }

        public async Task<Result<GetPlayerResponse>> GetPlayerById(Guid id)
        {
            var player = await unitOfWork.Repository<Player>()
                .FirstOrDefaultAsync(p => p.Id == id, default, p => p.Team);

            if (player == null)
            {
                return Result<GetPlayerResponse>.FailureStatusCode("Player not found", ErrorType.NotFound);
            }

            var response = new GetPlayerResponse
            {
                Id = player.Id,
                FullName = player.FullName,
                NickName = player.NickName,
                Position = player.Position,
                JerseyNumber = player.JerseyNumber,
                IsCaptain = player.IsCaptain,
                TeamId = player.TeamId,
                TeamName = player.Team?.Name ?? "N/A"
            };

            return Result<GetPlayerResponse>.Success(response);
        }

        public async Task<Result<IEnumerable<GetPlayerResponse>>> GetAllPlayers()
        {
            var playersQuery = unitOfWork.Repository<Player>().GetAll();
            
            var players = await playersQuery
                .Include(p => p.Team)
                .Select(p => new GetPlayerResponse
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    NickName = p.NickName,
                    Position = p.Position,
                    JerseyNumber = p.JerseyNumber,
                    IsCaptain = p.IsCaptain,
                    TeamId = p.TeamId,
                    TeamName = p.Team.Name
                })
                .ToListAsync();

            return Result<IEnumerable<GetPlayerResponse>>.Success(players);
        }
    }
}
