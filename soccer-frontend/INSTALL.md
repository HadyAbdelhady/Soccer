# Installation & Verification Guide

## Step 1: Prerequisites Check

Before starting, ensure you have:
- Node.js 18+ installed
- npm installed (comes with Node.js)
- Git (optional, for version control)

Verify installation:
```bash
node --version    # Should show v18.0.0 or higher
npm --version     # Should show 9.0.0 or higher
```

## Step 2: Navigate to Project

```bash
cd c:\Users\hady0\source\repos\Soccer\soccer-frontend
```

## Step 3: Install Dependencies

```bash
npm install
```

This will:
- Download all packages listed in `package.json`
- Install Angular 17, Material, RxJS, and other dependencies
- Create `node_modules/` directory
- Generate `package-lock.json`

**Expected time**: 2-5 minutes depending on internet speed

## Step 4: Verify Installation

```bash
# Check Angular CLI
ng version

# Should show:
# Angular CLI: 17.x.x
# Angular: 17.x.x
# TypeScript: 5.2.x
```

## Step 5: Start Development Server

```bash
npm start
```

Or use:
```bash
ng serve --open
```

**Expected output**:
```
⠙ Building...
✔ Build successful
Application bundle generation complete. [X.XX seconds]

Watch mode enabled. Watching for file changes.
●■■■■■■■■■■■■■■■■■■■ |  ES2022 modules transformation

Local:   http://localhost:4200/
```

## Step 6: Verify Application Loads

1. Browser should automatically open to `http://localhost:4200`
2. You should see the login page
3. No console errors (check browser console: F12)

## Step 7: Test Features

### Test Login Page
- Navigate to `http://localhost:4200/login`
- Form should load with email and password fields
- Validation should work (try submitting empty form)

### Test Navigation
- Try accessing `/dashboard` without logging in
- Should redirect to login (auth guard working)

### Test API Configuration
- Open `src/environments/environment.ts`
- Verify `apiUrl` is correctly set:
  ```typescript
  export const environment = {
    production: false,
    apiUrl: 'http://localhost:5000/api'
  };
  ```

## Troubleshooting

### Issue: "ng: The term 'ng' is not recognized"
**Solution**: 
```bash
npm install -g @angular/cli@17
```

### Issue: Port 4200 already in use
**Solution**:
```bash
ng serve --port 4201
```

### Issue: Module not found errors
**Solution**:
```bash
# Clear cache and reinstall
rm -rf node_modules package-lock.json
npm install
npm start
```

### Issue: CORS errors from API
**Solution**:
- Check backend API is running on port 5000
- Ensure backend has CORS enabled for `http://localhost:4200`
- Update API URL in `environment.ts` if needed

### Issue: Material icons not loading
**Solution**:
- Browser should load from Google CDN automatically
- Check internet connection
- Clear browser cache

## Project Structure Verification

Verify the following folders exist:
```
soccer-frontend/
├── src/
│   ├── app/
│   │   ├── core/              ✓
│   │   ├── shared/            ✓
│   │   └── features/          ✓
│   ├── environments/          ✓
│   ├── styles/                ✓
│   ├── index.html             ✓
│   └── main.ts                ✓
├── node_modules/              ✓ (after npm install)
├── package.json               ✓
├── angular.json               ✓
└── tsconfig.json              ✓
```

## Common Commands

```bash
# Development
npm start                    # Start dev server
npm run watch               # Watch for changes

# Building
npm run build              # Build for development
npm run build:prod         # Build for production

# Testing & Linting
npm test                   # Run unit tests
npm run lint              # Run linter

# Component/Service Generation
ng generate component features/example/pages/example-page
ng generate service core/services/example
```

## Environment Configuration

### Development
File: `src/environments/environment.ts`
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

### Production
File: `src/environments/environment.prod.ts`
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.soccermanager.com/api'
};
```

To use production config:
```bash
npm run build:prod
```

## Backend API Setup

Ensure your backend is running:
```bash
# Backend should be on http://localhost:5000
# API endpoints on http://localhost:5000/api
```

## Next Steps

1. ✅ Install dependencies: `npm install`
2. ✅ Start dev server: `npm start`
3. ✅ Verify app loads at `http://localhost:4200`
4. 📝 Start API integration
5. 🚀 Develop features
6. 📦 Build for production: `npm run build:prod`

## Testing the Application

### Login Flow
1. Try accessing any protected route (e.g., `/dashboard`)
2. Should redirect to `/login`
3. Form validation should prevent empty submissions

### Form Validation
1. Navigate to `/teams/create`
2. Try submitting empty form - should show validation errors
3. Fill in required fields - submit button should enable

### Notifications
1. Make any API call that fails
2. Toast notification should appear in top-right

## Documentation

- **README.md** - Full documentation
- **QUICKSTART.md** - Quick reference
- **SETUP.md** - Installation guide
- **PROJECT_FILES.md** - Complete file structure

## Support Resources

- [Angular Documentation](https://angular.io/docs)
- [Angular Material](https://material.angular.io/)
- [RxJS Documentation](https://rxjs.dev/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)

## Production Deployment

1. Build the project:
   ```bash
   npm run build:prod
   ```

2. Output will be in `dist/soccer-frontend/`

3. Deploy to your server:
   - Apache/Nginx: Configure to serve `index.html` for all routes
   - Cloud: Firebase, Vercel, Netlify, AWS S3, etc.

4. Update `environment.prod.ts` with production API URL

## Verification Checklist

- [ ] Node.js and npm installed
- [ ] Dependencies installed (`npm install` completed)
- [ ] Dev server starts (`npm start` works)
- [ ] App loads at `http://localhost:4200`
- [ ] No console errors
- [ ] Login page loads
- [ ] Auth guard prevents access to protected routes
- [ ] API URL configured correctly
- [ ] Material Design loads correctly
- [ ] Navigation works

Once all items are checked, your application is ready for development!
