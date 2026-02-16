# Frontend-Backend Integration Notes

## Current Status

The frontend dashboard is fully functional and ready to connect with the backend API. However, there are some GET endpoints that need to be added to the backend for the frontend to work optimally.

## Missing Backend Endpoints

The following endpoints are called by the frontend but are not currently in the backend. These need to be added to the backend:

### 1. **GET /api/tournament** - List all tournaments
**Used by:** Tournaments page (`/dashboard/tournaments`)
**Expected Response:**
```json
[
  {
    "id": "guid",
    "name": "Tournament Name",
    "type": "MULTI_GROUP_KNOCKOUT",
    "startDate": "2026-06-01T00:00:00Z",
    "endDate": "2026-07-15T00:00:00Z",
    "status": "ONGOING",
    "createdAt": "2026-02-15T00:00:00Z",
    "updatedAt": "2026-02-15T00:00:00Z"
  }
]
```

### 2. **GET /api/tournament/{id}** - Get tournament by ID
**Used by:** Tournament detail page (`/dashboard/tournaments/[id]`)
**Current Implementation:** May need expansion

### 3. **GET /api/team** - List all teams
**Used by:** Teams page (`/dashboard/teams`), Player creation forms
**Expected Response:**
```json
[
  {
    "id": "guid",
    "name": "Team Name",
    "country": "Country",
    "logo": "url",
    "founded": 2000,
    "createdAt": "2026-02-15T00:00:00Z",
    "updatedAt": "2026-02-15T00:00:00Z"
  }
]
```

### 4. **GET /api/team/{id}** - Get team by ID
**Used by:** Team detail page (`/dashboard/teams/[id]`)

### 5. **GET /api/player** - List all players (already exists)
**Status:** ✅ Implemented in backend

### 6. **GET /api/group?tournamentId={id}** - Get groups by tournament ID
**Used by:** Tournament detail page to show groups
**Current:** Uses `/api/group/tournament/{tournamentId}` which returns a single group
**Needed:** Endpoint should return array of groups for a tournament

### 7. **GET /api/match/getAllMatches** - List all matches (already exists)
**Status:** ✅ Implemented in backend

## Backend Changes Required

### Option 1: Add Missing Endpoints to Backend

Add these controller actions to the respective controllers:

#### TournamentController.cs
```csharp
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
```

#### TeamController.cs
```csharp
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
```

#### GroupController.cs - Update the tournament endpoint
```csharp
[HttpGet("tournament/{tournamentId}")]
[TranslateResultToActionResult]
public async Task>> GetGroupsByTournament(Guid tournamentId)
{
    // Return an array of groups instead of single group
    var result = await groupService.GetGroupsByTournament(tournamentId);
    return result;
}
```

### Option 2: Frontend Workaround (Temporary)

If you cannot modify the backend immediately, the frontend can:

1. **Cache created items:** Store newly created tournaments/teams in localStorage
2. **Use detail endpoints:** Fetch individual items instead of lists (less efficient)
3. **Populate from related data:** Extract teams from tournament data, etc.

## Alternative API Patterns

The backend uses a Result<T> wrapper pattern:
```csharp
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string[] Errors { get; set; }
    public string Message { get; set; }
}
```

This is correctly handled by the frontend API client, which expects responses in format:
```json
{
    "isSuccess": true,
    "data": {...},
    "errors": [],
    "message": ""
}
```

## Frontend Configuration

The frontend is already configured to:
- ✅ Handle JWT authentication via Bearer tokens
- ✅ Automatically include auth headers
- ✅ Parse Result<T> wrapper responses
- ✅ Store tokens in localStorage
- ✅ Redirect to login on 401 errors

## Testing the Integration

1. **Start the backend:**
   ```bash
   cd Soccer
   dotnet run
   ```

2. **Start the frontend:**
   ```bash
   npm run dev
   ```

3. **Test login:**
   - Navigate to http://localhost:3000/login
   - Use credentials from your backend setup
   - Should redirect to /dashboard on success

4. **Test data operations:**
   - Create a tournament
   - Create teams
   - Create players
   - All should persist to backend database

## Common Issues and Solutions

| Issue | Solution |
|-------|----------|
| "Failed to connect to server" | Verify backend is running on port 5000 |
| 404 on tournament list | Add GET /api/tournament endpoint to backend |
| CORS errors | Configure CORS in backend appsettings.json |
| 401 Unauthorized | Token expired, logout and login again |
| Form submission fails silently | Check browser console for API error responses |

## Next Steps

1. Add the missing GET endpoints to the backend
2. Test each endpoint with the frontend
3. Verify all CRUD operations work correctly
4. Consider adding request logging/debugging
5. Implement error notifications in the UI

## Support

For issues with the backend API, refer to:
- Backend Repository: https://github.com/HadyAbdelhady/Soccer
- Backend Documentation: Check the Soccer/README.md

For frontend issues, check:
- Network tab in browser DevTools
- Console logs for error messages
- `/lib/api.ts` for API client configuration
