# Backend Integration Guide

This frontend application is designed to work with the Soccer Tournament Management ASP.NET Core backend API.

## Backend Repository

The backend application is located at: https://github.com/HadyAbdelhady/Soccer

## API Configuration

The frontend is configured to connect to the backend at:
```
http://localhost:5000/api
```

To use a different API endpoint, update the `API_BASE_URL` in `/lib/api.ts`:

```typescript
const API_BASE_URL = 'http://your-backend-url/api'
```

## Backend Setup Instructions

1. Clone the backend repository:
```bash
git clone https://github.com/HadyAbdelhady/Soccer.git
cd Soccer
```

2. Set up the database (follow instructions in the backend repository)

3. Configure your `.env` or `appsettings.json` files with:
   - Database connection string
   - JWT secret key
   - CORS settings (add your frontend URL)

4. Run the backend application:
```bash
dotnet run
```

The API will be available at `http://localhost:5000`

## Available API Endpoints

### Authentication
- **POST** `/api/auth/login` - Login with username and password
- **POST** `/api/auth/logout` - Logout (requires Bearer token)
- **POST** `/api/auth/register/watcher` - Register a watcher account
- **PATCH** `/api/auth/me/fcm-token` - Update FCM token

### Tournaments
- **GET** `/api/tournament` - Get all tournaments
- **POST** `/api/tournament` - Create a new tournament
- **PATCH** `/api/tournament` - Update tournament
- **DELETE** `/api/tournament/{id}` - Delete tournament
- **POST** `/api/tournament/{id}/groups/draw` - Generate groups
- **POST** `/api/tournament/{id}/matches/draw` - Generate matches
- **GET** `/api/tournament/{id}/top-scorers` - Get top scorers
- **GET** `/api/tournament/{id}/stats` - Get tournament statistics

### Teams
- **GET** `/api/team` - Get all teams
- **POST** `/api/team` - Create a new team
- **PATCH** `/api/team` - Update team
- **DELETE** `/api/team/{id}` - Delete team

### Players
- **GET** `/api/player` - Get all players
- **GET** `/api/player/{id}` - Get player by ID
- **POST** `/api/player` - Create a new player
- **PATCH** `/api/player` - Update player
- **DELETE** `/api/player/{id}` - Delete player

### Groups
- **GET** `/api/group/tournament/{tournamentId}` - Get groups by tournament
- **GET** `/api/group/{id}/standings` - Get group standings
- **POST** `/api/group` - Create a new group
- **PATCH** `/api/group` - Update group
- **DELETE** `/api/group/{id}` - Delete group
- **POST** `/api/group/assign-teams` - Assign teams to group

### Matches
- **GET** `/api/match/getAllMatches` - Get all matches
- **POST** `/api/match` - Create a new match
- **POST** `/api/match/{id}/result` - Submit match result
- **POST** `/api/match/{id}/goals` - Add goal to match
- **POST** `/api/match/{id}/cards` - Add card to match
- **POST** `/api/match/{id}/lineup` - Set match lineup
- **GET** `/api/match/{id}/lineup` - Get match lineup
- **PATCH** `/api/match/{id}/schedule` - Update match schedule

## Request/Response Examples

### Login
**Request:**
```json
POST /api/auth/login
{
  "username": "admin",
  "password": "password123"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "username": "admin",
      "email": "admin@example.com",
      "role": "Admin"
    }
  }
}
```

### Create Tournament
**Request:**
```json
POST /api/tournament
{
  "name": "World Cup 2026",
  "type": "MULTI_GROUP_KNOCKOUT",
  "startDate": "2026-06-01T00:00:00Z",
  "endDate": "2026-07-15T00:00:00Z"
}
```

### Create Team
**Request:**
```json
POST /api/team
{
  "name": "Manchester United",
  "country": "England",
  "logo": "https://example.com/logo.png"
}
```

### Create Player
**Request:**
```json
POST /api/player
{
  "teamId": "team-id-here",
  "name": "John Doe",
  "shirtNumber": 10,
  "position": "MIDFIELDER",
  "dateOfBirth": "1990-05-15T00:00:00Z",
  "nationality": "English"
}
```

## Authentication

The API uses JWT (JSON Web Tokens) for authentication. After logging in, the token is stored in localStorage and automatically included in all subsequent requests via the `Authorization: Bearer {token}` header.

## Troubleshooting

- **Connection Refused**: Ensure the backend is running on `http://localhost:5000`
- **CORS Errors**: Update backend CORS settings to include your frontend URL
- **Authentication Failed**: Verify credentials match those in the backend database
- **Token Expired**: The frontend will automatically clear the token and redirect to login

## Development

For development, you can use VS Code REST Client to test endpoints:

Create a file called `test.http` with:
```http
@baseUrl = http://localhost:5000/api
@token = your_jwt_token_here

### Login
POST {{baseUrl}}/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password"
}

### Get All Tournaments
GET {{baseUrl}}/tournament
Authorization: Bearer {{token}}
```
