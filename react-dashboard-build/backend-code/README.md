# Backend Code to Add to Soccer Project

Follow these steps to add the GetAll functionality:

## 1. Add DTOs

Copy these files to your Soccer project:
- `GetAllTeamsResponse.cs` → `Infrastructure/DTOs/Teams/`
- `GetAllTournamentsResponse.cs` → `Infrastructure/DTOs/Tournaments/`

## 2. Update Service Interfaces

- Open `Infrastructure/Services/Teams/ITeamService.cs`
  - Add the method from `ITeamService_Update.cs`

- Open `Infrastructure/Services/Tournaments/ITournamentService.cs`
  - Add the method from `ITournamentService_Update.cs`

## 3. Implement Service Methods

- Open `Infrastructure/Services/Teams/TeamService.cs`
  - Add the method from `TeamService_GetAllTeams.cs`

- Open `Infrastructure/Services/Tournaments/TournamentService.cs`
  - Add the method from `TournamentService_GetAllTournaments.cs`

## 4. Add Controller Endpoints

- Open `Soccer/Controllers/TeamController.cs`
  - Add the endpoint from `TeamController_GetAllTeams.cs`

- Open `Soccer/Controllers/TournamentController.cs`
  - Add the endpoint from `TournamentController_GetAllTournaments.cs`

## 5. Build and Run

```bash
dotnet build
dotnet run
```

## 6. Test Endpoints

Use Swagger UI (usually at `http://localhost:5252/`) to test:
- `GET /api/team`
- `GET /api/tournament`

Once the backend is running with these changes, the frontend will automatically work!
