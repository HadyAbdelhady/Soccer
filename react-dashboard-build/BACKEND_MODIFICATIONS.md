# Backend Modifications Required

This guide explains the exact changes needed in the backend to achieve 100% frontend functionality.

## Summary

The frontend is calling for these GET endpoints that don't currently exist in the backend:

1. `GET /api/tournament` - List all tournaments
2. `GET /api/tournament/{id}` - Get tournament by ID  
3. `GET /api/team` - List all teams
4. `GET /api/team/{id}` - Get team by ID

## Implementation Guide

### File 1: Soccer/Controllers/TournamentController.cs

**Add these two methods to the TournamentController class:**

```csharp
/// <summary>
/// Get all tournaments
/// </summary>
[HttpGet]
[TranslateResultToActionResult]
public async Task>> GetAllTournaments()
{
    var result = await tournamentService.GetAllTournaments();
    return result;
}

/// <summary>
/// Get tournament by ID
/// </summary>
[HttpGet("{id}")]
[TranslateResultToActionResult]
public async Task> GetTournamentById(Guid id)
{
    var result = await tournamentService.GetTournamentById(id);
    return result;
}
```

**Location:** After the existing CreateTournament method

**Full updated controller:**
```csharp
using Business.DTOs.Tournaments;
using Business.Services.Standings;
using Business.Services.Tournaments;
using Infra.ResultWrapper;
using Microsoft.AspNetCore.Mvc;
using Soccer.Filters;

namespace Soccer.Controllers
{
 [Route("api/[controller]")]
 [ApiController]
 public class TournamentController(ITournamentService tournamentService, IStandingsService standingsService) : ControllerBase
 {
 private readonly ITournamentService tournamentService = tournamentService;
 private readonly IStandingsService _standingsService = standingsService;

 [HttpGet]
 [TranslateResultToActionResult]
 public async Task>> GetAllTournaments()
 {
 var result = await tournamentService.GetAllTournaments();
 return result;
 }

 [HttpGet("{id}")]
 [TranslateResultToActionResult]
 public async Task> GetTournamentById(Guid id)
 {
 var result = await tournamentService.GetTournamentById(id);
 return result;
 }

 [HttpPost]
 [TranslateResultToActionResult]
 public async Task > CreateTournament(CreateTournamentRequest request)
 {
 var result = await tournamentService.CreateTournament(request);
 return result;
 }

 [HttpPost("addTeamsToTournament")]
 [TranslateResultToActionResult]
 public async Task >> AddTeamsToTournament([FromBody] AddTeamsToTournamentRequest request)
 {
 return await tournamentService.AddTeamsToTournament(request);
 }

 [HttpPost("{id}/groups/draw")]
 [TranslateResultToActionResult]
 public async Task > GenerateGroups(Guid id)
 {
 var result = await tournamentService.GenerateGroupsAsync(id);
 return result;
 }

 [HttpPost("{id}/groups/regenerate")]
 [TranslateResultToActionResult]
 public async Task > RegenerateGroups(Guid id)
 {
 var result = await tournamentService.RegenerateGroupsAsync(id);
 return result;
 }

 [HttpPost("{id}/matches/draw")]
 [TranslateResultToActionResult]
 public async Task > GenerateMatches(Guid id)
 {
 var result = await tournamentService.GenerateMatchesAsync(id);
 return result;
 }

 [HttpPost("{id}/matches/regenerate")]
 [TranslateResultToActionResult]
 public async Task > RegenerateMatches(Guid id)
 {
 var result = await tournamentService.RegenerateMatchesAsync(id);
 return result;
 }

 [HttpPost("{id}/reset-schedule")]
 [TranslateResultToActionResult]
 public async Task > ResetSchedule(Guid id)
 {
 var result = await tournamentService.ResetScheduleAsync(id);
 return result;
 }

 [HttpPatch]
 [TranslateResultToActionResult]
 public async Task > UpdateTournament(UpdateTournamentRequest request)
 {
 var result = await tournamentService.UpdateTournament(request);
 return result;
 }

 [HttpDelete("{id}")]
 [TranslateResultToActionResult]
 public async Task > DeleteTournament(Guid id)
 {
 var result = await tournamentService.DeleteTournament(id);
 return result;
 }

 [HttpGet("{id}/top-scorers")]
 [TranslateResultToActionResult]
 public async Task >> GetTopScorers(Guid id, [FromQuery] int? topN)
 {
 return await _standingsService.GetTournamentTopScorersAsync(id, topN);
 }

 [HttpGet("{id}/stats")]
 [TranslateResultToActionResult]
 public async Task > GetTournamentStats(Guid id)
 {
 return await _standingsService.GetTournamentStatsAsync(id);
 }

 [HttpGet("{id}/player-stats")]
 [TranslateResultToActionResult]
 public async Task >> GetTournamentPlayerStats(Guid id)
 {
 return await _standingsService.GetTournamentPlayerStatsAsync(id);
 }
 }
}
```

### File 2: Soccer/Controllers/TeamController.cs

**Add these two methods to the TeamController class:**

```csharp
/// <summary>
/// Get all teams
/// </summary>
[HttpGet]
[TranslateResultToActionResult]
public async Task>> GetAllTeams()
{
    var result = await teamService.GetAllTeams();
    return result;
}

/// <summary>
/// Get team by ID
/// </summary>
[HttpGet("{id}")]
[TranslateResultToActionResult]
public async Task> GetTeamById(Guid id)
{
    var result = await teamService.GetTeamById(id);
    return result;
}
```

**Location:** Before the CreateTeam method

**Full updated controller:**
```csharp
using Business.DTOs.Teams;
using Business.Services.Teams;
using Infra.ResultWrapper;
using Microsoft.AspNetCore.Mvc;
using Soccer.Filters;

namespace Soccer.Controllers
{
 [Route("api/[controller]")]
 [ApiController]
 public class TeamController(ITeamService teamService) : ControllerBase
 {
 private readonly ITeamService teamService = teamService;

 [HttpGet]
 [TranslateResultToActionResult]
 public async Task>> GetAllTeams()
 {
 var result = await teamService.GetAllTeams();
 return result;
 }

 [HttpGet("{id}")]
 [TranslateResultToActionResult]
 public async Task> GetTeamById(Guid id)
 {
 var result = await teamService.GetTeamById(id);
 return result;
 }

 [HttpPost]
 [TranslateResultToActionResult]
 public async Task > CreateTeam(CreateTeamRequest request)
 {
 var result = await teamService.CreateTeam(request);
 return result;
 }

 [HttpPatch]
 [TranslateResultToActionResult]
 public async Task > UpdateTeam(UpdateTeamRequest request)
 {
 var result = await teamService.UpdateTeam(request);
 return result;
 }

 [HttpDelete("{id}")]
 [TranslateResultToActionResult]
 public async Task > DeleteTeam(Guid id)
 {
 var result = await teamService.DeleteTeam(id);
 return result;
 }
 }
}
```

## Service Layer Changes

You need to add these methods to your service interfaces and implementations:

### ITournamentService

```csharp
Task>> GetAllTournaments();
Task> GetTournamentById(Guid id);
```

### TournamentService (Implementation)

```csharp
public async Task>> GetAllTournaments()
{
    try
    {
        var tournaments = await _dbContext.Tournaments
            .AsNoTracking()
            .ToListAsync();
        
        var dtos = _mapper.Map<List<TournamentDto>>(tournaments);
        return Result.Success(dtos);
    }
    catch (Exception ex)
    {
        return Result.FailureStatusCode($"Error retrieving tournaments: {ex.Message}", ErrorType.ServerError);
    }
}

public async Task> GetTournamentById(Guid id)
{
    try
    {
        var tournament = await _dbContext.Tournaments
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (tournament == null)
            return Result.FailureStatusCode("Tournament not found", ErrorType.NotFound);
        
        var dto = _mapper.Map<TournamentDto>(tournament);
        return Result.Success(dto);
    }
    catch (Exception ex)
    {
        return Result.FailureStatusCode($"Error retrieving tournament: {ex.Message}", ErrorType.ServerError);
    }
}
```

### ITeamService

```csharp
Task>> GetAllTeams();
Task> GetTeamById(Guid id);
```

### TeamService (Implementation)

```csharp
public async Task>> GetAllTeams()
{
    try
    {
        var teams = await _dbContext.Teams
            .AsNoTracking()
            .ToListAsync();
        
        var dtos = _mapper.Map<List<TeamDto>>(teams);
        return Result.Success(dtos);
    }
    catch (Exception ex)
    {
        return Result.FailureStatusCode($"Error retrieving teams: {ex.Message}", ErrorType.ServerError);
    }
}

public async Task> GetTeamById(Guid id)
{
    try
    {
        var team = await _dbContext.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (team == null)
            return Result.FailureStatusCode("Team not found", ErrorType.NotFound);
        
        var dto = _mapper.Map<TeamDto>(team);
        return Result.Success(dto);
    }
    catch (Exception ex)
    {
        return Result.FailureStatusCode($"Error retrieving team: {ex.Message}", ErrorType.ServerError);
    }
}
```

## Optional: Improve GroupController

The GroupController already has `/api/group/tournament/{tournamentId}` endpoint, but you might want to verify it returns a list:

```csharp
[HttpGet("tournament/{tournamentId}")]
[TranslateResultToActionResult]
public async Task> GetGroupsByTournament(Guid tournamentId)
{
    // Make sure this returns List<GroupDto>, not just GroupDto
    var result = await groupService.GetGroupsByTournament(tournamentId);
    return result;
}
```

## Testing the Changes

### Using VS Code REST Client

Create a file `test.http`:

```http
@baseUrl = http://localhost:5000/api
@adminToken = your_admin_jwt_token_here

### Get All Tournaments
GET {{baseUrl}}/tournament
Authorization: Bearer {{adminToken}}

### Get Specific Tournament
GET {{baseUrl}}/tournament/your-tournament-id-here
Authorization: Bearer {{adminToken}}

### Get All Teams
GET {{baseUrl}}/team
Authorization: Bearer {{adminToken}}

### Get Specific Team
GET {{baseUrl}}/team/your-team-id-here
Authorization: Bearer {{adminToken}}
```

### Using Postman

1. Create GET request to: `http://localhost:5000/api/tournament`
2. Add Authorization header: `Bearer {token}`
3. Send request
4. Should return list of tournaments

## Deployment Considerations

After making these changes:

1. **Test locally first**
   - Restart backend server
   - Login from frontend
   - Verify data loads

2. **Commit changes**
   - Add to version control
   - Create pull request if using team workflow

3. **Run migrations if needed**
   - No database changes required
   - Only adding new endpoints

4. **Verify CORS**
   - Ensure frontend URL is in allowed origins
   - Test from frontend after deployment

## Rollback Plan

If you need to revert:

1. Remove the added methods from controllers
2. Remove the service methods
3. Restart backend
4. Frontend will gracefully handle missing endpoints

## Expected Frontend Behavior After Changes

Once these endpoints are added:

1. ✅ Tournament list page loads with data
2. ✅ Team selection works in player forms
3. ✅ Tournament detail pages load properly
4. ✅ All features work end-to-end

## Support

If you encounter issues:

1. Check backend error logs
2. Verify database has data
3. Ensure mappings are correct (TournamentDto, TeamDto)
4. Test endpoints with Postman/REST Client first
5. Check frontend console for error details

## Time Required

Adding these endpoints should take:
- 15-20 minutes for service methods
- 5-10 minutes for controller methods
- 10-15 minutes for testing
- **Total: 30-45 minutes**

## Completion Checklist

- [ ] Added GetAllTournaments to TournamentController
- [ ] Added GetTournamentById to TournamentController
- [ ] Added GetAllTeams to TeamController
- [ ] Added GetTeamById to TeamController
- [ ] Implemented service methods
- [ ] Added service methods to interface
- [ ] Tested with REST client
- [ ] Verified frontend can now fetch lists
- [ ] Confirmed no errors in backend logs
- [ ] Tested with frontend application

---

Once these changes are complete, your soccer tournament management system will be fully functional! ⚽
