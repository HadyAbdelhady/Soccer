# Soccer Frontend - Angular Application

## Quick Start

```bash
cd soccer-frontend
npm install
npm start
```

The application will be available at `http://localhost:4200`

## API Configuration

The application is configured to connect to the backend API at `http://localhost:5000/api` by default.

To change this, edit `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://your-api-url/api'
};
```

## Default Credentials

For testing, use the default test credentials (check with your backend team).

## Features Implemented

### Authentication
- Login page with form validation
- Signup page with registration
- JWT token management
- Automatic token refresh
- Logout functionality

### Dashboard
- Tournament overview
- Match summary
- Quick actions

### Teams
- View all teams
- Create new teams
- Edit team information
- Delete teams

### Players
- List all players
- Create player profiles
- Edit player information
- Delete players

### Tournaments
- View all tournaments
- Create tournaments
- Edit tournament details
- Tournament types: League, Knockout, Mixed

### Matches
- View scheduled matches
- Match details with scores
- Team lineups
- Goals and cards tracking

### Groups
- View tournament groups
- Group standings
- Top scorers

## Architecture

- **Standalone Components**: Using Angular 17 standalone component API
- **Reactive Forms**: FormBuilder with validation
- **RxJS Observables**: Reactive state management
- **HTTP Interceptors**: JWT token injection, error handling
- **Guards**: Authentication and role-based access control
- **Material Design**: Professional UI components

## Development

### Add a new feature

1. Create components under `src/app/features/{feature-name}/`
2. Create services under `src/app/core/services/`
3. Add routing in feature module
4. Import feature module in main routes

### Add a new service

1. Create under `src/app/core/services/`
2. Inject dependencies
3. Export from `src/app/core/services/index.ts`

## Build for Production

```bash
npm run build:prod
```

This generates optimized production build in `dist/soccer-frontend/`

## Deployment

1. Build the application: `npm run build:prod`
2. Deploy `dist/soccer-frontend/` to your web server
3. Configure server to redirect all routes to `index.html`

## Troubleshooting

### Port 4200 already in use

```bash
ng serve --port 4201
```

### CORS Issues

Make sure your backend API allows CORS requests from `http://localhost:4200`

### Module not found errors

Run `npm install` again to ensure all dependencies are installed

## Support

For detailed documentation, see [README.md](./README.md)
