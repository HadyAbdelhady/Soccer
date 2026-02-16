'use client'

import { createContext, useContext, useEffect, useState } from 'react'
import { api } from '@/lib/api'
import type { User } from '@/lib/types'

interface AuthContextType {
  user: User | null
  loading: boolean
  isAuthenticated: boolean
  login: (username: string, password: string) => Promise<boolean>
  logout: () => void
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    // Check if user is stored in localStorage
    const storedUser = localStorage.getItem('user')
    const token = localStorage.getItem('auth_token')

    if (storedUser && token) {
      try {
        setUser(JSON.parse(storedUser))
        api.setToken(token)
      } catch (error) {
        localStorage.removeItem('user')
        localStorage.removeItem('auth_token')
      }
    }

    setLoading(false)
  }, [])

  const login = async (username: string, password: string): Promise<boolean> => {
    setLoading(true)
    try {
      const response = await api.post('/auth/login', { username, password })

      if (response.isSuccess && response.data) {
        // Backend returns a flat LoginResponse object with accessToken
        const loginData = response.data as any
        const token = loginData.accessToken || loginData.AccessToken

        if (!token) {
          console.error('No access token found in login response')
          return false
        }

        // Construct User object from available fields
        // Backend LoginResponse: { id, name, username, role, accessToken, refreshToken, message }
        // Frontend User: { id, username, email, role, teamId?, createdAt, updatedAt }
        const user: User = {
          id: loginData.id || loginData.Id,
          username: loginData.username || loginData.Username,
          role: loginData.role || loginData.Role,
          // Missing fields in backend response, using placeholders
          email: '', // Not returned by backend login
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }

        api.setToken(token)
        localStorage.setItem('user', JSON.stringify(user))
        setUser(user)
        return true
      }

      return false
    } finally {
      setLoading(false)
    }
  }

  const logout = () => {
    api.clearToken()
    setUser(null)
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        isAuthenticated: !!user,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider')
  }
  return context
}
