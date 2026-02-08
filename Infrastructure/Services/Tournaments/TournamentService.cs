using Business.DTOs.Tournaments;
using Infra.ResultWrapper;
using Infra.Interface;
using Infra.enums;
using Data.Entities;

namespace Business.Services.Tournaments
{
    public class TournamentService(IUnitOfWork unitOfWork) : ITournamentService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<Result<CreateTournamentResponse>> CreateTournament(CreateTournamentRequest request)
        {
            var tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                StartDateTime = request.StartDate,
                EndDateTime = request.EndDate,
                CreatedAt = DateTimeOffset.UtcNow
            };
            var result = new CreateTournamentResponse
            {
                Id = tournament.Id,
                Name = tournament.Name
            };
            await unitOfWork.Repository<Tournament>().AddAsync(tournament);
            await unitOfWork.SaveChangesAsync();
            return Result<CreateTournamentResponse>.Success(result);
        }

        public async Task<Result<UpdateTournamentResponse>> UpdateTournament(UpdateTournamentRequest request)
        {
            var tournament = await unitOfWork.Repository<Tournament>().GetByIdAsync(request.Id);
            
            if (tournament == null || tournament.IsDeleted)
            {
                return Result<UpdateTournamentResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            tournament.Name = request.Name;
            tournament.StartDateTime = request.StartDate;
            tournament.EndDateTime = request.EndDate;
            tournament.UpdatedAt = DateTimeOffset.UtcNow;

            unitOfWork.Repository<Tournament>().Update(tournament);
            await unitOfWork.SaveChangesAsync();

            var response = new UpdateTournamentResponse
            {
                Message = "Updated successfully",
                Id = tournament.Id,
                Name = tournament.Name,
                StartDate = tournament.StartDateTime,
                EndDate = tournament.EndDateTime
            };

            return Result<UpdateTournamentResponse>.Success(response);
        }

        public async Task<Result<DeleteTournamentResponse>> DeleteTournament(Guid id)
        {
            var tournament = await unitOfWork.Repository<Tournament>().GetByIdAsync(id);

            if (tournament == null || tournament.IsDeleted)
            {
                return Result<DeleteTournamentResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            tournament.UpdatedAt = DateTimeOffset.UtcNow;
            unitOfWork.Repository<Tournament>().Remove(tournament);
            await unitOfWork.SaveChangesAsync();

            var response = new DeleteTournamentResponse
            {
                Id = id,
                IsDeleted = true,
                Message = "Deleted successfully"
            };

            return Result<DeleteTournamentResponse>.Success(response);
        }
    }
}
