// Add this endpoint to TeamController.cs in Soccer/Controllers/

[HttpGet]
[TranslateResultToActionResult]
public async Task<Result<GetAllTeamsResponse>> GetAllTeams()
{
    return await teamService.GetAllTeams();
}
