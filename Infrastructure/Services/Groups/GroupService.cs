using Business.DTOs.Groups;
using Infra.ResultWrapper;
using Infra.Interface;
using Infra.enums;
using Data.Entities;

namespace Business.Services.Groups
{
    public class GroupService(IUnitOfWork unitOfWork) : IGroupService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

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
                TournamentId = group.TournamentId
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
                Message = "Updated successfully",
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
                Message = "Deleted successfully"
            };

            return Result<DeleteGroupResponse>.Success(response);
        }
    }
}
