using Business.DTOs.Teams;
using Infra.ResultWrapper;
using Infra.Interface;
using Infra.enums;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.Teams
{
    public class TeamService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher) : ITeamService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IPasswordHasher passwordHasher = passwordHasher;

        public async Task<Result<CreateTeamResponse>> CreateTeam(CreateTeamRequest request)
        {
            var baseUsername = request.Name.ToLower().Replace(" ", "_");
            var uniqueSuffix = Guid.NewGuid().ToString()[..4];
            var generatedUsername = $"{baseUsername}_{uniqueSuffix}";

            var generatedPassword = GenerateRandomPassword(12);
            var hashedPassword = passwordHasher.HashPassword(generatedPassword);

            var team = new TeamUser
            {
                Id = Guid.NewGuid(),
                FullName = request.Name,
                Username = generatedUsername,
                HashedPassword = hashedPassword,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await unitOfWork.Repository<TeamUser>().AddAsync(team);
            await unitOfWork.SaveChangesAsync();

            var result = new CreateTeamResponse
            {
                Id = team.Id,
                Name = team.FullName,
                Username = generatedUsername,
                Password = generatedPassword,
                Message = "Created Successfully"
            };

            return Result<CreateTeamResponse>.Success(result);
        }

        public async Task<Result<UpdateTeamResponse>> UpdateTeam(UpdateTeamRequest request)
        {
            var team = await unitOfWork.Repository<TeamUser>().GetByIdAsync(request.Id);

            if (team == null)
            {
                return Result<UpdateTeamResponse>.FailureStatusCode("Team not found", ErrorType.NotFound);
            }

            team.FullName = request.Name;
            team.UpdatedAt = DateTimeOffset.UtcNow;

            unitOfWork.Repository<TeamUser>().Update(team);
            await unitOfWork.SaveChangesAsync();

            var response = new UpdateTeamResponse
            {
                Message = "Updated Successfully",
                Id = team.Id,
                Name = team.FullName
            };

            return Result<UpdateTeamResponse>.Success(response);
        }

        public async Task<Result<DeleteTeamResponse>> DeleteTeam(Guid id)
        {
            var team = await unitOfWork.Repository<TeamUser>().GetByIdAsync(id);

            if (team == null)
            {
                return Result<DeleteTeamResponse>.FailureStatusCode("Team not found", ErrorType.NotFound);
            }

            team.UpdatedAt = DateTimeOffset.UtcNow;
            unitOfWork.Repository<TeamUser>().Remove(team);
            await unitOfWork.SaveChangesAsync();

            var response = new DeleteTeamResponse
            {
                Id = id,
                IsDeleted = true,
                Message = "Deleted Successfully"
            };

            return Result<DeleteTeamResponse>.Success(response);
        }

        public async Task<Result<GetAllTeamsResponse>> GetAllTeams()
        {
            var teams = await unitOfWork.Repository<TeamUser>().GetAll().ToListAsync();
            
            var teamDtos = teams.Select(t => new GetTeamDto
            {
                Id = t.Id,
                Name = t.FullName,
                Username = t.Username
            }).ToList();

            return Result<GetAllTeamsResponse>.Success(new GetAllTeamsResponse
            {
                Teams = teamDtos
            });
        }

        public async Task<Result<GetTeamsNotInTournamentsResponse>> GetTeamsNotInTournaments()
        {
            var teamsNotInTournaments = await unitOfWork.Repository<TeamUser>()
                .GetAll()
                .Where(t => !t.Tournaments.Any())
                .ToListAsync();
            
            var teamDtos = teamsNotInTournaments.Select(t => new GetTeamDto
            {
                Id = t.Id,
                Name = t.FullName,
                Username = t.Username
            }).ToList();

            return Result<GetTeamsNotInTournamentsResponse>.Success(new GetTeamsNotInTournamentsResponse
            {
                Teams = teamDtos
            });
        }

        private static string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%^&*?";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
