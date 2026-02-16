// Add this method to TournamentService.cs in Infrastructure/Services/Tournaments/

public async Task<Result<GetAllTournamentsResponse>> GetAllTournaments()
{
    var tournaments = await _unitOfWork.Repository<Tournament>().GetAllAsync();
    
    var tournamentDtos = tournaments.Select(t => new GetTournamentDto
    {
        Id = t.Id,
        Name = t.Name,
        Type = t.Type,
        StartDate = t.StartDate,
        EndDate = t.EndDate
    }).ToList();

    return Result<GetAllTournamentsResponse>.Success(new GetAllTournamentsResponse
    {
        Tournaments = tournamentDtos
    });
}
