# Soccer Manager - Angular Frontend

A production-ready Angular 17+ frontend application for the Soccer Tournament Management System.

## Features

- **Authentication System**: Login, signup, JWT token management
- **User Management**: User roles and permissions
- **Teams Management**: Create, edit, delete teams
- **Players Management**: Manage player information and team assignments
- **Tournaments**: Create and manage tournaments with groups
- **Matches**: Track match schedules, scores, goals, and cards
- **Responsive Design**: Material Design UI with responsive layout
- **Real-time Updates**: Observable-based state management
- **Error Handling**: Global error interceptor with user feedback

## Project Structure

```
src/
├── app/
│   ├── core/                    # Core functionality
│   │   ├── guards/              # Auth and role guards
│   │   ├── interceptors/        # HTTP interceptors
│   │   ├── models/              # TypeScript interfaces
│   │   └── services/            # Business logic services
│   ├── shared/                  # Shared components and utilities
│   │   ├── components/          # Reusable components
│   │   ├── models/              # Shared models
│   │   └── pipes/               # Custom pipes
│   ├── features/                # Feature modules
│   │   ├── auth/                # Authentication
│   │   ├── dashboard/           # Dashboard
│   │   ├── teams/               # Teams management
│   │   ├── players/             # Players management
│   │   ├── tournaments/         # Tournaments
│   │   ├── matches/             # Matches
│   │   └── groups/              # Groups management
│   ├── app.routes.ts            # Application routing
│   └── app.component.ts         # Root component
├── environments/                # Environment configuration
├── styles/                      # Global styles
└── index.html                   # Entry point
```

## Installation

### Prerequisites
- Node.js 18+
- npm or pnpm

### Setup

1. Clone the repository:
```bash
cd soccer-frontend
```

2. Install dependencies:
```bash
npm install
# or
pnpm install
```

3. Set up environment variables:
   - Update `src/environments/environment.ts` with your API URL (default: `http://localhost:5000/api`)

## Development

Start the development server:

```bash
npm start
# or
ng serve
```

Navigate to `http://localhost:4200`. The app will automatically reload if you change source files.

## Building

Build for production:

```bash
npm run build:prod
# or
ng build --configuration production
```

The build artifacts will be stored in the `dist/` directory.

## Services

### Core Services

- **AuthService**: Authentication and user state management
- **ApiService**: HTTP client wrapper with base URL configuration
- **StorageService**: LocalStorage abstraction for tokens
- **NotificationService**: Toast notification system
- **TeamService**: Team CRUD operations
- **PlayerService**: Player management
- **TournamentService**: Tournament management
- **MatchService**: Match operations

## Models

All TypeScript models matching the backend are defined in `src/app/core/models/`:
- User types (Team, Admin, Viewer)
- Entities: Team, Player, Tournament, Group, Match, Goal, Card, Lineup
- Enums: UserRole, PlayerPosition, MatchStatus, CardType, GoalType, etc.
- API response wrappers: Result<T>, PaginatedResult<T>

## Authentication

- JWT-based authentication
- Token stored in localStorage
- Automatic token attachment to requests via interceptor
- Protected routes with AuthGuard
- Role-based access control with RoleGuard

## Material Design

The application uses Angular Material for UI components:
- Material toolbar, buttons, forms
- Material dialog for confirmations
- Material tables for data display
- Material cards for layouts
- Material icons

## Interceptors

- **JwtInterceptor**: Automatically attaches JWT token to requests
- **ErrorInterceptor**: Global error handling with user notifications

## Lazy Loading

Feature modules are lazy loaded for better performance:
- Auth module
- Dashboard module
- Teams module
- Players module
- Tournaments module
- Matches module
- Groups module

## Environment Configuration

### Development (`environment.ts`)
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

### Production (`environment.prod.ts`)
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.soccermanager.com/api'
};
```

## Testing

Run unit tests:

```bash
npm test
```

## Linting

Run linter:

```bash
npm run lint
```

## Contributing

1. Create a feature branch (`git checkout -b feature/AmazingFeature`)
2. Commit changes (`git commit -m 'Add AmazingFeature'`)
3. Push to branch (`git push origin feature/AmazingFeature`)
4. Open a Pull Request

## License

This project is licensed under the MIT License.

## Support

For support, please contact the development team or create an issue in the repository.
