using Business.DTOs.Teams;
using Infra.ResultWrapper;
using Infra.Interface;
using Infra.enums;
using Data.Entities;

namespace Business.Services.Teams
{
    public class TeamService(IUnitOfWork unitOfWork) : ITeamService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<Result<CreateTeamResponse>> CreateTeam(CreateTeamRequest request)
        {
            var baseUsername = request.Name.ToLower().Replace(" ", "_");
            var uniqueSuffix = Guid.NewGuid().ToString()[..4];
            var generatedUsername = $"{baseUsername}_{uniqueSuffix}";

            var generatedPassword = GenerateRandomPassword(12);

            var team = new Team
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Username = generatedUsername,
                HashedPassword = generatedPassword,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await unitOfWork.Repository<Team>().AddAsync(team);
            await unitOfWork.SaveChangesAsync();

            var result = new CreateTeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                Username = generatedUsername,
                Password = generatedPassword,
                Message = "Created Successfully"
            };

            return Result<CreateTeamResponse>.Success(result);
        }

        public async Task<Result<UpdateTeamResponse>> UpdateTeam(UpdateTeamRequest request)
        {
            var team = await unitOfWork.Repository<Team>().GetByIdAsync(request.Id);

            if (team == null)
            {
                return Result<UpdateTeamResponse>.FailureStatusCode("Team not found", ErrorType.NotFound);
            }

            team.Name = request.Name;
            team.UpdatedAt = DateTimeOffset.UtcNow;

            unitOfWork.Repository<Team>().Update(team);
            await unitOfWork.SaveChangesAsync();

            var response = new UpdateTeamResponse
            {
                Message = "Updated Successfully",
                Id = team.Id,
                Name = team.Name
            };

            return Result<UpdateTeamResponse>.Success(response);
        }

        public async Task<Result<DeleteTeamResponse>> DeleteTeam(Guid id)
        {
            var team = await unitOfWork.Repository<Team>().GetByIdAsync(id);

            if (team == null)
            {
                return Result<DeleteTeamResponse>.FailureStatusCode("Team not found", ErrorType.NotFound);
            }

            team.UpdatedAt = DateTimeOffset.UtcNow;
            unitOfWork.Repository<Team>().Remove(team);
            await unitOfWork.SaveChangesAsync();

            var response = new DeleteTeamResponse
            {
                Id = id,
                IsDeleted = true,
                Message = "Deleted Successfully"
            };

            return Result<DeleteTeamResponse>.Success(response);
        }

        public async Task<Result<LoginResponse>> Login(LoginRequest request)
        {
            var team = await unitOfWork.Repository<Team>()
                .FirstOrDefaultAsync(t => t.Username == request.Username);

            if (team == null || team.HashedPassword != request.Password)
            {
                return Result<LoginResponse>.FailureStatusCode("Invalid username or password", ErrorType.UnAuthorized);
            }

            var response = new LoginResponse
            {
                Id = team.Id,
                Name = team.Name,
                Username = team.Username,
                Message = "Login successfully"
            };

            return Result<LoginResponse>.Success(response);
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
