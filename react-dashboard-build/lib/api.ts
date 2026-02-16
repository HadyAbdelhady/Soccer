const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api'

export interface ApiResponse<T> {
  isSuccess: boolean
  data?: T
  errors?: string[]
  message?: string
}

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
  user: {
    id: string
    username: string
    email: string
    role: 'Team' | 'Admin' | 'Viewer'
    teamId?: string
  }
}

class ApiClient {
  private token: string | null = null

  constructor() {
    if (typeof window !== 'undefined') {
      this.token = localStorage.getItem('auth_token')
    }
  }

  setToken(token: string) {
    this.token = token
    if (typeof window !== 'undefined') {
      localStorage.setItem('auth_token', token)
    }
  }

  clearToken() {
    this.token = null
    if (typeof window !== 'undefined') {
      localStorage.removeItem('auth_token')
      localStorage.removeItem('user')
    }
  }

  getToken() {
    return this.token
  }

  private async request<T>(
    method: string,
    endpoint: string,
    data?: any,
  ): Promise<ApiResponse<T>> {
    const url = `${API_BASE_URL}${endpoint}`
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
    }

    if (this.token) {
      headers['Authorization'] = `Bearer ${this.token}`
    }

    const options: RequestInit = {
      method,
      headers,
    }

    if (data && (method === 'POST' || method === 'PATCH' || method === 'PUT')) {
      options.body = JSON.stringify(data)
    }

    try {
      const response = await fetch(url, options)
      let responseData

      try {
        responseData = await response.json()
      } catch (e) {
        // Handle non-JSON responses (e.g. 502 Bad Gateway HTML)
        return {
          isSuccess: false,
          errors: ['Invalid server response'],
        }
      }

      // Check if this is our backend's Result<T> format
      // Backend: { isSuccess: boolean, value: T, error: string }
      if (typeof responseData.isSuccess !== 'undefined') {
        const mappedResponse: ApiResponse<T> = {
          isSuccess: responseData.isSuccess,
          data: responseData.value,
          errors: responseData.error ? [responseData.error] : [],
          message: responseData.error || (responseData.isSuccess ? 'Success' : 'Operation failed')
        }
        return mappedResponse
      }

      // Fallback for other APIs or unhandled errors
      if (!response.ok) {
        return {
          isSuccess: false,
          errors: responseData.errors || [responseData.message || 'An error occurred'],
        }
      }

      return responseData
    } catch (error) {
      console.error('API Error:', error)
      return {
        isSuccess: false,
        errors: ['Failed to connect to server'],
      }
    }
  }

  async get<T>(endpoint: string): Promise<ApiResponse<T>> {
    return this.request<T>('GET', endpoint)
  }

  async post<T>(endpoint: string, data: any): Promise<ApiResponse<T>> {
    return this.request<T>('POST', endpoint, data)
  }

  async patch<T>(endpoint: string, data: any): Promise<ApiResponse<T>> {
    return this.request<T>('PATCH', endpoint, data)
  }

  async delete<T>(endpoint: string): Promise<ApiResponse<T>> {
    return this.request<T>('DELETE', endpoint)
  }

  async getTeamsNotInTournaments(): Promise<ApiResponse<any[]>> {
    return this.get<any[]>('/team/not-in-tournaments')
  }

  async addTeamsToTournament(tournamentId: string, teamIds: string[]): Promise<ApiResponse<any[]>> {
    return this.post<any[]>('/tournament/addTeamsToTournament', {
      tournamentId,
      teamIds
    })
  }

  async resetSchedule(tournamentId: string): Promise<ApiResponse<any>> {
    return this.post<any>(`/tournament/${tournamentId}/reset-schedule`, {})
  }

  async getTournamentsWithTeamCount(): Promise<ApiResponse<any>> {
    return this.get<any>('/tournament/with-team-count')
  }

  async generateGroups(tournamentId: string): Promise<ApiResponse<any>> {
    return this.post<any>(`/tournament/${tournamentId}/groups/draw`, {})
  }

  async generateMatches(tournamentId: string): Promise<ApiResponse<any>> {
    return this.post<any>(`/tournament/${tournamentId}/matches/draw`, {})
  }
}

export const api = new ApiClient()
