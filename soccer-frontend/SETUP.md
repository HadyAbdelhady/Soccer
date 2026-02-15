# Project Setup Instructions

## Prerequisites Installation

### Windows

1. **Install Node.js 18+**
   - Download from https://nodejs.org/
   - Run installer and follow prompts
   - Verify installation: `node --version` and `npm --version`

2. **Install Angular CLI**
   ```bash
   npm install -g @angular/cli@17
   ```

3. **Verify Angular CLI**
   ```bash
   ng version
   ```

## Setup Soccer Frontend

1. Navigate to the project:
   ```bash
   cd c:\Users\hady0\source\repos\Soccer\soccer-frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Update API URL (if needed):
   - Edit `src/environments/environment.ts`
   - Change `apiUrl` to your backend URL

4. Start development server:
   ```bash
   npm start
   ```

5. Open browser and navigate to:
   ```
   http://localhost:4200
   ```

## Running the Application

### Development Mode
```bash
npm start
```
- Application will reload automatically on code changes
- Source maps enabled for debugging
- Opens browser automatically

### Production Build
```bash
npm run build:prod
```
- Output: `dist/soccer-frontend/`
- Optimized and minified

### Watch Mode
```bash
npm run watch
```
- Watches for changes but doesn't serve

## Project Structure

```
soccer-frontend/
├── src/
│   ├── app/                 # Application code
│   │   ├── core/           # Core services, guards, interceptors
│   │   ├── shared/         # Reusable components
│   │   └── features/       # Feature modules
│   ├── environments/       # Environment config
│   ├── styles/             # Global styles
│   ├── index.html          # Entry point
│   └── main.ts             # Bootstrap file
├── angular.json            # Angular CLI config
├── tsconfig.json           # TypeScript config
├── package.json            # Dependencies
└── README.md               # Documentation
```

## Common Issues

### Port 4200 in use
```bash
ng serve --port 4201
```

### Module not found
```bash
npm install
rm -rf node_modules/.angular
```

### Cannot find module errors
- Run: `npm install`
- Rebuild: `npm start`

### API connection errors
- Check `environment.ts` API URL
- Ensure backend is running
- Check CORS configuration in backend

## Next Steps

1. Configure API endpoints
2. Run tests: `npm test`
3. Build for production: `npm run build:prod`
4. Deploy to server

## Useful Commands

```bash
# Install dependencies
npm install

# Start dev server
npm start

# Build for production
npm run build:prod

# Run tests
npm test

# Run linter
npm run lint

# Generate component
ng generate component features/example/pages/example-page

# Generate service
ng generate service core/services/example

# Generate module
ng generate module features/example
```

## Deployment

### Build
```bash
npm run build:prod
```

### Server Configuration (for Apache/Nginx)
Ensure all routes redirect to `index.html` for SPA compatibility

### Environment Setup for Production
Update `src/environments/environment.prod.ts` with:
- Production API URL
- Production settings

### Deploy
Copy `dist/soccer-frontend/` to your web server

## Support

For Angular documentation: https://angular.io/docs
For Angular Material: https://material.angular.io/
For RxJS: https://rxjs.dev/
