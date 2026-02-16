// Add this endpoint to TournamentController.cs in Soccer/Controllers/

[HttpGet]
[TranslateResultToActionResult]
public async Task<Result<GetAllTournamentsResponse>> GetAllTournaments()
{
    return await tournamentService.GetAllTournaments();
}
