// Add this method to TeamService.cs in Infrastructure/Services/Teams/

public async Task<Result<GetAllTeamsResponse>> GetAllTeams()
{
    var teams = await _unitOfWork.Repository<Team>().GetAllAsync();
    
    var teamDtos = teams.Select(t => new GetTeamDto
    {
        Id = t.Id,
        Name = t.Name,
        Username = t.Username
    }).ToList();

    return Result<GetAllTeamsResponse>.Success(new GetAllTeamsResponse
    {
        Teams = teamDtos
    });
}
