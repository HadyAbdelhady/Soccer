# Soccer Tournament Management Dashboard

A modern, full-featured web application for managing soccer tournaments, teams, players, matches, and standings. Built with Next.js 16, React 19, and TypeScript.

## Overview

This is the **frontend application** for a comprehensive soccer tournament management system. It provides an intuitive interface for tournament organizers to:

- Create and manage tournaments
- Register teams and manage rosters
- Schedule and track matches
- Record match results with goals and cards
- View group standings and statistics
- Generate group draws and match schedules

The frontend integrates with an ASP.NET Core backend API for all data persistence.

## Quick Start

### Prerequisites
- Node.js 18+ (with pnpm)
- Backend API running on `http://localhost:5000`

### Installation
```bash
# Install dependencies
pnpm install

# Copy environment template
cp .env.example .env.local

# Start development server
pnpm dev
```

Visit http://localhost:3000 and login with your backend credentials.

## Documentation

- **[QUICKSTART.md](./QUICKSTART.md)** - Get started in 5 minutes
- **[BACKEND_SETUP.md](./BACKEND_SETUP.md)** - Backend configuration and API endpoints
- **[INTEGRATION_NOTES.md](./INTEGRATION_NOTES.md)** - Backend integration details
- **[API_CLIENT.md](./API_CLIENT.md)** - API client usage guide

## Features

### Authentication & Authorization
- ✅ JWT-based authentication
- ✅ Role-based access control (Admin, Team, Viewer)
- ✅ Persistent login with localStorage
- ✅ Automatic token refresh

### Tournament Management
- ✅ Create tournaments with custom configurations
- ✅ Support for single-group and multi-group knockout formats
- ✅ Automatic group generation and team assignment
- ✅ Match schedule generation
- ✅ Tournament statistics and analytics

### Team Management
- ✅ Register teams with country/location info
- ✅ Manage team rosters
- ✅ Track team performance
- ✅ View team-specific statistics

### Player Management
- ✅ Add players to teams
- ✅ Assign positions (GK, DEF, MID, FWD)
- ✅ Track player information (age, nationality)
- ✅ Filter players by team
- ✅ Player statistics and performance tracking

### Match Management
- ✅ Schedule matches with dates and venues
- ✅ Record match results in real-time
- ✅ Track goals with minute and scorer information
- ✅ Record disciplinary actions (yellow/red cards)
- ✅ Set team lineups and substitutions
- ✅ Match event timeline

### Group & Standings
- ✅ Automatic group standings calculation
- ✅ Real-time point updates
- ✅ Goal difference tracking
- ✅ League table views by group
- ✅ Head-to-head comparisons

### Analytics & Statistics
- ✅ Top scorers rankings
- ✅ Tournament-wide statistics
- ✅ Per-player performance metrics
- ✅ Team performance analysis
- ✅ Historical data tracking

## Tech Stack

### Frontend
- **Next.js 16** - React framework with App Router
- **React 19** - UI library
- **TypeScript** - Type safety
- **Tailwind CSS** - Utility-first CSS
- **shadcn/ui** - Component library
- **React Hook Form** - Form management
- **SWR** - Data fetching and caching

### Backend Integration
- **Axios/Fetch API** - HTTP client
- **JWT** - Authentication tokens
- **REST API** - Backend communication

## Project Structure

```
.
├── app/                          # Next.js App Router
│   ├── layout.tsx               # Root layout
│   ├── page.tsx                 # Home redirect
│   ├── login/                   # Login page
│   ├── dashboard/               # Dashboard layout
│   │   ├── page.tsx            # Dashboard home
│   │   ├── tournaments/         # Tournament pages
│   │   ├── teams/               # Team pages
│   │   ├── players/             # Player pages
│   │   ├── matches/             # Match pages
│   │   └── groups/              # Group standings pages
│   ├── globals.css              # Global styles
│   └── auth-context.tsx         # Auth state management
│
├── components/                   # Reusable components
│   ├── ui/                      # shadcn/ui components
│   ├── tournament-form.tsx      # Tournament creation form
│   ├── team-form.tsx            # Team registration form
│   ├── player-form.tsx          # Player registration form
│   ├── match-scoresheet.tsx     # Match details view
│   ├── group-standings.tsx      # League table component
│   └── loading-skeleton.tsx     # Loading states
│
├── lib/                         # Utilities
│   ├── api.ts                   # API client
│   ├── types.ts                 # TypeScript definitions
│   └── hooks.ts                 # Custom React hooks
│
├── public/                      # Static assets
├── tailwind.config.ts           # Tailwind configuration
├── tsconfig.json                # TypeScript configuration
└── package.json                 # Dependencies
```

## API Integration

The application connects to an ASP.NET Core backend at:
```
http://localhost:5000/api
```

### Available Endpoints

**Authentication**
- POST `/api/auth/login` - User login
- POST `/api/auth/logout` - User logout

**Tournaments**
- GET `/api/tournament` - List tournaments
- POST `/api/tournament` - Create tournament
- PATCH `/api/tournament` - Update tournament
- DELETE `/api/tournament/{id}` - Delete tournament

**Teams**
- GET `/api/team` - List teams
- POST `/api/team` - Create team
- PATCH `/api/team` - Update team
- DELETE `/api/team/{id}` - Delete team

**Players**
- GET `/api/player` - List players
- POST `/api/player` - Create player
- PATCH `/api/player` - Update player
- DELETE `/api/player/{id}` - Delete player

**Matches**
- GET `/api/match/getAllMatches` - List matches
- POST `/api/match` - Create match
- POST `/api/match/{id}/result` - Submit match result
- POST `/api/match/{id}/goals` - Add goal
- POST `/api/match/{id}/cards` - Add card

**Groups**
- GET `/api/group/tournament/{id}` - Get tournament groups
- GET `/api/group/{id}/standings` - Get group standings
- POST `/api/group` - Create group

See [BACKEND_SETUP.md](./BACKEND_SETUP.md) for complete API documentation.

## Authentication Flow

1. User navigates to `/login`
2. Enters username and password
3. Frontend sends POST request to `/api/auth/login`
4. Backend returns JWT token in response
5. Token is stored in localStorage
6. All subsequent requests include `Authorization: Bearer {token}` header
7. Automatic redirect to `/dashboard` on successful login
8. Logout clears token and redirects to login

## Development

### Running the Development Server
```bash
pnpm dev
```
Application runs at http://localhost:3000 with Hot Module Replacement (HMR).

### Building for Production
```bash
pnpm build
pnpm start
```

### Code Quality
```bash
pnpm lint              # Run ESLint
pnpm format            # Format code with Prettier
```

## Environment Variables

Create `.env.local`:
```env
# Backend API URL
NEXT_PUBLIC_API_URL=http://localhost:5000/api

# For production
# NEXT_PUBLIC_API_URL=https://api.yourdomain.com
```

## Styling

The application uses:
- **Tailwind CSS** for utility-first styling
- **CSS Variables** for theming (light/dark modes)
- **shadcn/ui** component library built on Radix UI

Dark theme is enabled by default. Customize colors in:
- `app/globals.css` - CSS variables and theme
- `tailwind.config.ts` - Tailwind configuration

## State Management

- **React Context** - Authentication state (user, token)
- **React Hooks** - Component-level state
- **SWR** - Server state and data caching
- **localStorage** - Persistent authentication

## Error Handling

The application includes:
- Global error boundaries
- API error handling with user-friendly messages
- Form validation with error display
- Network error detection and retry logic
- Automatic logout on 401 responses

## Performance

- Server-Side Rendering (SSR) with Next.js
- Static Generation (SSG) where applicable
- Image optimization
- Code splitting and lazy loading
- CSS and JavaScript minification
- Font optimization

## Accessibility

- Semantic HTML structure
- ARIA labels and roles
- Keyboard navigation support
- Screen reader friendly
- Color contrast compliance
- Focus management

## Security

- JWT token-based authentication
- HTTPOnly cookie support (optional)
- CORS protection
- Input validation and sanitization
- XSS prevention with React escaping
- CSRF token support (if needed)

## Browser Support

- Chrome/Edge (latest 2 versions)
- Firefox (latest 2 versions)
- Safari (latest 2 versions)
- Mobile browsers (iOS Safari, Chrome Android)

## Known Limitations

The backend needs the following GET endpoints for full functionality:
- `GET /api/tournament` - List all tournaments
- `GET /api/team` - List all teams

See [INTEGRATION_NOTES.md](./INTEGRATION_NOTES.md) for details and workarounds.

## Deployment

### Vercel (Recommended)
```bash
# Connect GitHub repository
# Auto-deploy on push
# Set NEXT_PUBLIC_API_URL environment variable in Vercel dashboard
```

### Self-Hosted
```bash
pnpm build
pnpm start
```

Use process manager (PM2) or Docker for production deployment.

### Docker
```dockerfile
FROM node:18-alpine
WORKDIR /app
COPY package.json pnpm-lock.yaml ./
RUN npm install -g pnpm && pnpm install
COPY . .
RUN pnpm build
EXPOSE 3000
CMD ["pnpm", "start"]
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Troubleshooting

### "Failed to connect to server"
- Ensure backend is running on `http://localhost:5000`
- Check `NEXT_PUBLIC_API_URL` in `.env.local`
- Verify firewall allows port 5000

### "Invalid username or password"
- Verify credentials match those in backend database
- Ensure user account has been created in backend
- Check user role is properly assigned

### CORS errors
- Configure CORS in backend `appsettings.json`
- Add frontend URL to allowed origins: `http://localhost:3000`

### Build errors
- Clear `.next` folder: `rm -rf .next`
- Reinstall dependencies: `pnpm install`
- Restart development server

## Support

- **Documentation:** See [QUICKSTART.md](./QUICKSTART.md) and [BACKEND_SETUP.md](./BACKEND_SETUP.md)
- **Issues:** Create an issue on GitHub
- **Backend:** https://github.com/HadyAbdelhady/Soccer

## License

This project is part of the Soccer Tournament Management System.

## Changelog

### v1.0.0 (2026-02-15)
- Initial release
- Complete dashboard UI
- Tournament management
- Team and player management
- Match scheduling and scoring
- Group standings display
- Authentication system

## Credits

- Built with [Next.js](https://nextjs.org/)
- UI Components from [shadcn/ui](https://ui.shadcn.com/)
- Styling with [Tailwind CSS](https://tailwindcss.com/)
- Backend by [HadyAbdelhady](https://github.com/HadyAbdelhady)

---

**Happy Tournament Managing! ⚽**
