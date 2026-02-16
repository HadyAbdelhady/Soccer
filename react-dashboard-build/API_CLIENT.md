# API Client Documentation

The frontend uses a custom `ApiClient` class located in `/lib/api.ts` for all backend communication. This document explains how to use it.

## Configuration

### API Base URL

The client connects to:
```
http://localhost:5000/api
```

To change the API endpoint, update `NEXT_PUBLIC_API_URL`:

```bash
# .env.local
NEXT_PUBLIC_API_URL=https://api.yourdomain.com
```

## Basic Usage

### Importing the Client

```typescript
import { api } from '@/lib/api'
```

### Methods

The API client provides four main methods:

#### GET Request
```typescript
const response = await api.get<DataType>('/endpoint')

// Example
const tournaments = await api.get<Tournament[]>('/tournament')
if (tournaments.isSuccess && tournaments.data) {
  // Handle success
  console.log(tournaments.data)
} else {
  // Handle error
  console.error(tournaments.errors)
}
```

#### POST Request
```typescript
const response = await api.post<ResponseType>('/endpoint', data)

// Example
const newTournament = await api.post<Tournament>('/tournament', {
  name: 'World Cup',
  type: 'MULTI_GROUP_KNOCKOUT',
  startDate: '2026-06-01',
  endDate: '2026-07-15'
})
```

#### PATCH Request
```typescript
const response = await api.patch<ResponseType>('/endpoint', data)

// Example
const updated = await api.patch<Tournament>('/tournament', {
  id: 'tournament-id',
  name: 'Updated Name'
})
```

#### DELETE Request
```typescript
const response = await api.delete<ResponseType>('/endpoint')

// Example
const deleted = await api.delete<Tournament>(`/tournament/${id}`)
```

## Response Format

All responses follow this structure:

```typescript
interface ApiResponse<T> {
  isSuccess: boolean
  data?: T
  errors?: string[]
  message?: string
}
```

### Successful Response
```json
{
  "isSuccess": true,
  "data": { /* your data */ },
  "errors": [],
  "message": "Operation successful"
}
```

### Error Response
```json
{
  "isSuccess": false,
  "data": null,
  "errors": ["Error message 1", "Error message 2"],
  "message": "Operation failed"
}
```

## Authentication

### Setting Token
```typescript
// After login
api.setToken(token)

// Token is automatically included in all subsequent requests
// Authorization: Bearer {token}
```

### Getting Token
```typescript
const token = api.getToken()
if (token) {
  // User is authenticated
}
```

### Clearing Token
```typescript
// After logout
api.clearToken()
```

## Common Patterns

### Loading States
```typescript
const [loading, setLoading] = useState(false)
const [data, setData] = useState<Tournament[]>([])

const fetchTournaments = async () => {
  setLoading(true)
  try {
    const response = await api.get<Tournament[]>('/tournament')
    if (response.isSuccess && response.data) {
      setData(response.data)
    }
  } catch (error) {
    console.error('Error:', error)
  } finally {
    setLoading(false)
  }
}
```

### Error Handling
```typescript
const response = await api.post<Team>('/team', formData)

if (response.isSuccess && response.data) {
  // Success
  setTeams([...teams, response.data])
  setError('')
} else {
  // Error
  const errorMessage = response.errors?.[0] || 'Failed to create team'
  setError(errorMessage)
}
```

### Data Validation
```typescript
if (!name || !city) {
  setError('Please fill in all required fields')
  return
}

const response = await api.post<Team>('/team', { name, city })
// ... handle response
```

## Custom Hooks

The `/lib/hooks.ts` file provides helper hooks:

### useFetch Hook
```typescript
import { useFetch } from '@/lib/hooks'

function MyComponent() {
  const { data: tournaments, loading, error } = useFetch<Tournament[]>('/tournament')
  
  return (
    <div>
      {loading && <div>Loading...</div>}
      {error && <div>Error: {error}</div>}
      {tournaments?.map(t => <div key={t.id}>{t.name}</div>)}
    </div>
  )
}
```

### useMutation Hook
```typescript
import { useMutation } from '@/lib/hooks'

function CreateTournamentForm() {
  const { mutate: createTournament, loading } = useMutation<Tournament>('/tournament', 'POST')
  
  const handleSubmit = async (formData) => {
    const response = await createTournament(formData)
    if (response.isSuccess) {
      // Success
    } else {
      // Error
    }
  }
  
  return (
    <button disabled={loading} onClick={handleSubmit}>
      {loading ? 'Creating...' : 'Create Tournament'}
    </button>
  )
}
```

## API Endpoints

### Authentication
```
POST   /api/auth/login
POST   /api/auth/logout
POST   /api/auth/register/watcher
PATCH  /api/auth/me/fcm-token
```

### Tournaments
```
GET    /api/tournament                    # List all (needs backend implementation)
GET    /api/tournament/{id}              # Get by ID
POST   /api/tournament                    # Create
PATCH  /api/tournament                    # Update
DELETE /api/tournament/{id}              # Delete
POST   /api/tournament/{id}/groups/draw
POST   /api/tournament/{id}/matches/draw
GET    /api/tournament/{id}/top-scorers
GET    /api/tournament/{id}/stats
GET    /api/tournament/{id}/player-stats
```

### Teams
```
GET    /api/team                          # List all (needs backend implementation)
GET    /api/team/{id}                    # Get by ID (needs backend implementation)
POST   /api/team                          # Create
PATCH  /api/team                          # Update
DELETE /api/team/{id}                    # Delete
```

### Players
```
GET    /api/player                        # List all
GET    /api/player/{id}                  # Get by ID
POST   /api/player                        # Create
PATCH  /api/player                        # Update
DELETE /api/player/{id}                  # Delete
```

### Groups
```
GET    /api/group/tournament/{id}         # Get by tournament
GET    /api/group/{id}/standings         # Get standings
POST   /api/group                         # Create
PATCH  /api/group                         # Update
DELETE /api/group/{id}                   # Delete
POST   /api/group/assign-teams           # Assign teams
```

### Matches
```
GET    /api/match/getAllMatches          # List all
POST   /api/match                         # Create
POST   /api/match/{id}/result            # Submit result
POST   /api/match/{id}/goals             # Add goal
POST   /api/match/{id}/cards             # Add card
POST   /api/match/{id}/lineup            # Set lineup
GET    /api/match/{id}/lineup            # Get lineup
PATCH  /api/match/{id}/schedule          # Update schedule
```

## Error Handling

### Network Errors
```typescript
try {
  const response = await api.get('/tournament')
  // Handle response
} catch (error) {
  // Network or parsing error
  console.error('Network error:', error)
}
```

### API Errors
```typescript
const response = await api.post('/tournament', data)

if (!response.isSuccess) {
  const errorMessages = response.errors || []
  const mainError = errorMessages[0] || 'Unknown error'
  console.error('API Error:', mainError)
}
```

### Validation Errors
The backend returns validation errors in the `errors` array:
```json
{
  "isSuccess": false,
  "errors": [
    "Tournament name is required",
    "Start date must be in the future"
  ]
}
```

## Best Practices

### 1. Always Check Success Status
```typescript
// ✅ Good
if (response.isSuccess && response.data) {
  // Use response.data
}

// ❌ Bad
const data = response.data  // May be undefined
```

### 2. Use TypeScript for Type Safety
```typescript
// ✅ Good
const response = await api.get<Tournament[]>('/tournament')

// ❌ Bad
const response = await api.get('/tournament')  // Unknown type
```

### 3. Handle Loading States
```typescript
// ✅ Good
{loading && <LoadingSpinner />}
{!loading && data && <DataDisplay data={data} />}

// ❌ Bad
<DataDisplay data={data} />  // May crash if data is undefined
```

### 4. Clean Error Messages
```typescript
// ✅ Good
setError(response.errors?.[0] || 'An error occurred')

// ❌ Bad
setError(JSON.stringify(response.errors))  // Raw data in UI
```

### 5. Use Proper HTTP Methods
```typescript
// ✅ Good
api.post(...)     // Create
api.patch(...)    // Update
api.delete(...)   // Delete

// ❌ Bad
api.post(...)     // For all operations
```

## Testing API Calls

### Using REST Client (VS Code)
Create `test.http`:
```http
@baseUrl = http://localhost:5000/api
@token = your_jwt_token

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

### Create Tournament
POST {{baseUrl}}/tournament
Content-Type: application/json
Authorization: Bearer {{token}}

{
  "name": "World Cup 2026",
  "type": "MULTI_GROUP_KNOCKOUT",
  "startDate": "2026-06-01T00:00:00Z",
  "endDate": "2026-07-15T00:00:00Z"
}
```

### Browser Console Testing
```javascript
// In browser DevTools console
import { api } from '/lib/api.js'

// Login
const loginRes = await api.post('/auth/login', {
  username: 'admin',
  password: 'password'
})

// Set token
api.setToken(loginRes.data.token)

// Get tournaments
const tournaments = await api.get('/tournament')
```

## Troubleshooting

| Issue | Cause | Solution |
|-------|-------|----------|
| 401 Unauthorized | Token expired/invalid | Re-login, check token validity |
| 404 Not Found | Endpoint doesn't exist | Add endpoint to backend |
| 500 Server Error | Backend error | Check backend logs |
| CORS Error | Cross-origin request blocked | Configure CORS in backend |
| Network Timeout | Backend unavailable | Verify backend is running |

## Migration to Production

When deploying to production:

1. **Update API URL:**
   ```bash
   NEXT_PUBLIC_API_URL=https://api.yourdomain.com
   ```

2. **Enable HTTPS:**
   - All tokens must be sent over HTTPS
   - Secure HttpOnly cookies recommended

3. **API Security:**
   - Implement rate limiting
   - Add request validation
   - Log suspicious activity
   - Monitor error rates

4. **Error Monitoring:**
   - Integrate Sentry or similar
   - Track 4xx and 5xx errors
   - Alert on error spikes
