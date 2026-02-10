using Business.DTOs.Groups;
using Infra.ResultWrapper;
using Infra.Interface;
using Infra.enums;
using Business.Services.Standings;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Infra.enums;

namespace Business.Services.Groups
{
    public class GroupService(IUnitOfWork unitOfWork, IStandingsService standingsService) : IGroupService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IStandingsService standingsService = standingsService;

        public async Task<Result<CreateGroupResponse>> CreateGroup(CreateGroupRequest request)
        {
            var tournament = await unitOfWork.Repository<Tournament>().GetByIdAsync(request.TournamentId);
            if (tournament == null)
            {
                return Result<CreateGroupResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                TournamentId = request.TournamentId,
                CreatedAt = DateTimeOffset.UtcNow
            };
            var result = new CreateGroupResponse
            {
                Id = group.Id,
                Name = group.Name,
                TournamentId = group.TournamentId,
                Message = "Created Successfully"
            };
            await unitOfWork.Repository<Group>().AddAsync(group);
            await unitOfWork.SaveChangesAsync();
            return Result<CreateGroupResponse>.Success(result);
        }

        public async Task<Result<UpdateGroupResponse>> UpdateGroup(UpdateGroupRequest request)
        {
            var group = await unitOfWork.Repository<Group>().GetByIdAsync(request.Id);

            if (group == null)
            {
                return Result<UpdateGroupResponse>.FailureStatusCode("Group not found", ErrorType.NotFound);
            }

            var tournament = await unitOfWork.Repository<Tournament>().GetByIdAsync(request.TournamentId);
            if (tournament == null)
            {
                return Result<UpdateGroupResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            group.Name = request.Name;
            group.TournamentId = request.TournamentId;
            group.UpdatedAt = DateTimeOffset.UtcNow;

            unitOfWork.Repository<Group>().Update(group);
            await unitOfWork.SaveChangesAsync();

            var response = new UpdateGroupResponse
            {
                Message = "Updated Successfully",
                Id = group.Id,
                Name = group.Name,
                TournamentId = group.TournamentId
            };

            return Result<UpdateGroupResponse>.Success(response);
        }

        public async Task<Result<DeleteGroupResponse>> DeleteGroup(Guid id)
        {
            var group = await unitOfWork.Repository<Group>().GetByIdAsync(id);

            if (group == null)
            {
                return Result<DeleteGroupResponse>.FailureStatusCode("Group not found", ErrorType.NotFound);
            }

            group.UpdatedAt = DateTimeOffset.UtcNow;
            unitOfWork.Repository<Group>().Remove(group);
            await unitOfWork.SaveChangesAsync();

            var response = new DeleteGroupResponse
            {
                Id = id,
                IsDeleted = true,
                Message = "Deleted Successfully"
            };

            return Result<DeleteGroupResponse>.Success(response);
        }

        public async Task<Result<TournamentGroupsResponseDto>> GetGroupsByTournament(Guid tournamentId)
        {
            var groupsData = await unitOfWork.Repository<Group>()
                .GetAll()
                .Include(g => g.Teams)
                .Where(g => g.TournamentId == tournamentId)
                .ToListAsync();

            var result = new TournamentGroupsResponseDto
            {
                TournamentId = tournamentId,
                Groups = []
            };

            foreach (var group in groupsData)
            {
                var standingsResult = await standingsService.GetGroupStandingsAsync(group.Id);
                var standings = standingsResult.IsSuccess ? standingsResult.Value!.Standings : [];

                result.Groups.Add(new GroupResponseDto
                {
                    Id = group.Id,
                    Name = group.Name,
                    Standings = standings
                });
            }

            return Result<TournamentGroupsResponseDto>.Success(result);
        }

        public async Task<Result<AssignTeamsResponse>> AssignTeamsToGroup(AssignTeamsRequest request)
        {
            var group = await unitOfWork.Repository<Group>().GetByIdAsync(request.GroupId);
            if (group == null)
            {
                return Result<AssignTeamsResponse>.FailureStatusCode("Group not found", ErrorType.NotFound);
            }

            var teams = await unitOfWork.Repository<Team>()
                .GetAll()
                .Where(t => request.TeamIds.Contains(t.Id))
                .ToListAsync();

            if (teams.Count == 0 && request.TeamIds.Count > 0)
            {
                return Result<AssignTeamsResponse>.FailureStatusCode("No valid teams found", ErrorType.NotFound);
            }

            foreach (var team in teams)
            {
                team.GroupId = request.GroupId;
                team.UpdatedAt = DateTimeOffset.UtcNow;
                unitOfWork.Repository<Team>().Update(team);
            }

            await unitOfWork.SaveChangesAsync();

            return Result<AssignTeamsResponse>.Success(new AssignTeamsResponse
            {
                Message = "Teams assigned successfully",
                AssignedCount = teams.Count
            });
        }
    }
}
