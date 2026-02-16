# Signup & Testing Guide

## Quick Start - Create Your Test Account

### Step 1: Open the Application
1. Make sure the frontend is running: `pnpm dev` (http://localhost:3000)
2. Make sure the backend is running: `dotnet run` (http://localhost:5000)

### Step 2: Go to Signup Page
1. Navigate to http://localhost:3000
2. You'll be redirected to the login page
3. Click **"Create Account"** button at the bottom

### Step 3: Fill in Your Details

```
First Name:      John
Last Name:       Doe
Email:           john@example.com
Username:        johndoe
Password:        password123
Confirm:         password123
```

**Password Requirements:**
- Minimum 6 characters
- Must match confirmation password
- No special requirements

### Step 4: Submit & Login
1. Click **"Sign Up"**
2. If successful, you'll be redirected to login
3. Enter your username and password
4. Click **"Sign In"**

## What Happens During Signup

When you submit the signup form, the application:

1. **Validates your input:**
   - All fields required
   - Valid email format
   - Username at least 3 characters
   - Password at least 6 characters
   - Passwords must match

2. **Sends to backend:**
   ```
   POST /api/auth/register/watcher
   {
     "firstName": "John",
     "lastName": "Doe",
     "email": "john@example.com",
     "username": "johndoe",
     "password": "password123"
   }
   ```

3. **Backend response:**
   - **Success:** User created, redirects to login
   - **Error:** Shows error message (user already exists, etc.)

## Testing Checklist

### Account Creation
- [ ] Create account with valid data
- [ ] See error when leaving fields empty
- [ ] See error for invalid email
- [ ] See error when passwords don't match
- [ ] See error for short password
- [ ] Successfully create account and redirect to login

### Login After Signup
- [ ] Login with newly created account
- [ ] See error with wrong password
- [ ] See error with non-existent user
- [ ] Successfully login and see dashboard

### Dashboard Access
- [ ] View dashboard home page
- [ ] See user info in sidebar
- [ ] Access all menu options
- [ ] Create tournament
- [ ] Create team
- [ ] Create player

## Common Issues & Solutions

### "Invalid request" on signup
**Cause:** Backend endpoint not returning expected format
**Solution:** Check backend is running and `/api/auth/register/watcher` endpoint exists

### "User already exists"
**Cause:** Username or email already registered
**Solution:** Try with different username/email

### Stuck on signup page after submission
**Cause:** Backend not responding
**Solution:** 
1. Check backend is running: `dotnet run`
2. Verify `NEXT_PUBLIC_API_URL` points to correct backend URL
3. Check browser console for error messages

### Password validation errors
**Cause:** Password doesn't match requirements
**Solution:** Ensure:
- Password is at least 6 characters
- Confirmation password matches exactly
- No typos

## Environment Configuration

The signup feature uses the backend API endpoint. Make sure:

1. **Backend is running:**
   ```bash
   cd Soccer/Soccer
   dotnet run
   ```
   Should show: `Application started`

2. **API URL is correct:**
   Check `.env.local` or use default:
   ```
   NEXT_PUBLIC_API_URL=http://localhost:5000/api
   ```

## Testing Different Scenarios

### Scenario 1: Basic Signup Flow
1. Click "Create Account"
2. Fill in all fields with valid data
3. Click "Sign Up"
4. Verify redirected to login
5. Login with new credentials
6. Verify on dashboard

### Scenario 2: Validation Testing
1. Try to signup with empty fields → Should show "required" error
2. Try with short password → Should show "at least 6 characters"
3. Try with mismatched passwords → Should show "do not match"
4. Try with invalid email → Should show "valid email"

### Scenario 3: Duplicate Account
1. Create first account (user: test1)
2. Try to create again with same username
3. Should see error about user already existing

### Scenario 4: Login After Signup
1. Create new account
2. Note down credentials
3. Logout (if needed)
4. Login with new credentials
5. Verify access to dashboard and all features

## Data Validation

All input is validated on both frontend and backend:

**Frontend Validation:**
- Real-time on input change
- Prevents submission with invalid data
- Shows helpful error messages

**Backend Validation:**
- Username uniqueness
- Email format and uniqueness
- Password strength
- User creation and token generation

## Next Steps

After successfully creating an account:

1. **Create a Tournament**
   - Go to Tournaments → Create Tournament
   - Fill in details and submit

2. **Create a Team**
   - Go to Teams → Register Team
   - Add team information

3. **Create Players**
   - Go to Players → Add Player
   - Assign to team and position

4. **Schedule Matches**
   - Go to Matches → Schedule Match
   - Select teams and date

5. **Record Match Results**
   - Go to Matches → Select Match
   - Add goals and cards
   - View updated standings

## Getting Help

If you encounter issues:

1. **Check the logs:**
   - Browser console (F12)
   - Backend console output

2. **Verify setup:**
   - Backend running on port 5000
   - Frontend running on port 3000
   - Database created and seeded

3. **Check documentation:**
   - README.md - Full setup guide
   - BACKEND_MODIFICATIONS.md - Backend setup details
   - INTEGRATION_SUMMARY.md - Architecture overview

## Technical Details

### Signup Form Structure
- **First Name:** Text input, required, string
- **Last Name:** Text input, required, string
- **Email:** Email input, required, must contain @
- **Username:** Text input, required, min 3 characters
- **Password:** Password input, required, min 6 characters
- **Confirm Password:** Password input, required, must match password

### Form Validation
- Real-time validation on change
- Prevents form submission if invalid
- Clear error messages
- Accessibility compliant

### Error Handling
- Network errors → "An error occurred during signup"
- Validation errors → Specific field message
- Backend errors → Server error message
- All errors shown in red alert box

## Success Criteria

Signup feature is working correctly when:

✅ Can create account with valid data
✅ Validation errors show for invalid data
✅ Redirected to login after successful signup
✅ Can login with newly created account
✅ Dashboard loads after login
✅ All tournament features accessible
✅ Can create tournaments, teams, players, matches
