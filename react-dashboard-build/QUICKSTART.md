# Quick Start Guide

## Prerequisites

- Node.js 18+ and npm/pnpm
- .NET 8 SDK
- SQL Server or compatible database

## Frontend Setup (This Project)

1. **Install dependencies:**
   ```bash
   pnpm install
   ```

2. **Configure environment (optional):**
   - Create `.env.local` file (copy from `.env.example`):
   ```bash
   cp .env.example .env.local
   ```
   - Update `NEXT_PUBLIC_API_URL` if your backend isn't running on `localhost:5000`

3. **Start development server:**
   ```bash
   pnpm dev
   ```
   - Frontend will be available at: http://localhost:3000

## Backend Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/HadyAbdelhady/Soccer.git
   cd Soccer
   ```

2. **Follow backend setup instructions:**
   - See `Soccer/README.md` for database setup
   - Configure connection strings in `appsettings.json`
   - Setup JWT secrets and authentication

3. **Run migrations (if using EF Core):**
   ```bash
   dotnet ef database update
   ```

4. **Start backend:**
   ```bash
   cd Soccer
   dotnet run
   ```
   - Backend will be available at: http://localhost:5000
   - API docs (if Swagger enabled): http://localhost:5000/swagger

## First Time Setup

After starting both frontend and backend:

1. **Access the login page:**
   - Navigate to: http://localhost:3000/login

2. **Login with backend credentials:**
   - Use credentials you created during backend setup
   - Or use default credentials if configured in backend

3. **You're in!**
   - The dashboard should load successfully
   - You can now start managing tournaments

## Common Setup Issues

### Backend Won't Start
```
Problem: "Connection string not set" or database errors
Solution: 
- Check appsettings.json has correct connection string
- Ensure database exists and is accessible
- Run: dotnet ef database update
```

### Frontend Can't Connect to Backend
```
Problem: "Failed to connect to server" errors in login
Solution:
- Verify backend is running on http://localhost:5000
- Check NEXT_PUBLIC_API_URL in .env.local
- Ensure no firewall is blocking port 5000
- Check CORS is enabled in backend
```

### Login Fails
```
Problem: "Invalid username or password"
Solution:
- Verify you're using correct credentials from backend
- Check user account exists in backend database
- Ensure user has proper role assigned (Admin, Team, etc.)
```

### CORS Errors in Browser Console
```
Problem: "Access to XMLHttpRequest blocked by CORS policy"
Solution in Backend appsettings.json:
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:3001"],
    "AllowedMethods": ["GET", "POST", "PATCH", "DELETE"],
    "AllowedHeaders": ["Content-Type", "Authorization"]
  }
}
```

## Project Structure

### Frontend (`/app`)
```
app/
├── page.tsx                    # Redirects to login
├── login/                      # Login page
├── dashboard/
│   ├── page.tsx               # Main dashboard
│   ├── tournaments/           # Tournament management
│   ├── teams/                 # Team management
│   ├── players/               # Player management
│   ├── matches/               # Match management
│   └── groups/                # Group standings
├── auth-context.tsx           # Authentication context
└── layout.tsx                 # Root layout
```

### Components (`/components`)
```
components/
├── tournament-form.tsx        # Create/edit tournament
├── team-form.tsx              # Create/edit team
├── player-form.tsx            # Create/edit player
├── match-scoresheet.tsx       # Match details view
├── group-standings.tsx        # League table display
└── loading-skeleton.tsx       # Loading states
```

### Libraries (`/lib`)
```
lib/
├── api.ts                     # API client
├── types.ts                   # TypeScript interfaces
└── hooks.ts                   # Custom hooks
```

## Features

### Implemented
- ✅ User authentication with JWT
- ✅ Tournament management (CRUD)
- ✅ Team management (CRUD)
- ✅ Player management (CRUD)
- ✅ Match scheduling and scoring
- ✅ Group standings display
- ✅ Match lineups and events
- ✅ Tournament statistics

### Requires Backend Endpoints
- ⏳ GET all tournaments
- ⏳ GET all teams
- ⏳ GET tournament by ID
- ⏳ GET team by ID

See `INTEGRATION_NOTES.md` for details on adding missing endpoints.

## Development Tips

### Hot Reload
- Frontend changes auto-reload in browser
- Backend changes require manual restart

### Debugging
- **Frontend:** Open DevTools (F12) → Network/Console tabs
- **Backend:** Check console output and log files
- **API:** Use REST client (VS Code extension) to test endpoints

### Environment Variables
- `NEXT_PUBLIC_*` variables are exposed to browser
- Use for frontend-accessible configs only
- Backend URLs, API keys go in `NEXT_PUBLIC_API_URL`

## Deployment

### Frontend (Vercel)
```bash
# Connect GitHub repository
# Vercel auto-deploys on push
# Set NEXT_PUBLIC_API_URL environment variable
```

### Backend (Azure/AWS/Docker)
- Package as Docker container
- Deploy to your preferred platform
- Update `NEXT_PUBLIC_API_URL` in frontend

## Next Steps

1. Add missing GET endpoints to backend (see INTEGRATION_NOTES.md)
2. Test all CRUD operations
3. Setup proper error handling and logging
4. Configure production database
5. Deploy to staging for testing
6. Configure analytics/monitoring

## Support

- **Frontend Issues:** Check console logs, network tab
- **Backend Issues:** Check backend logs, database connections
- **Integration:** See BACKEND_SETUP.md and INTEGRATION_NOTES.md
- **GitHub:** https://github.com/HadyAbdelhady/Soccer

## Commands Reference

```bash
# Frontend
pnpm dev              # Start dev server
pnpm build            # Build for production
pnpm start            # Start production build
pnpm lint             # Run linter

# Backend
dotnet run            # Start dev server
dotnet build          # Build project
dotnet test           # Run tests
dotnet ef database update    # Run migrations
```

Enjoy building your Soccer Tournament Manager! ⚽
