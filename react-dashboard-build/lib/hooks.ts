import { useState, useEffect } from 'react'
import { api } from '@/lib/api'
import type { ApiResponse } from '@/lib/api'

export function useFetch<T>(endpoint: string) {
  const [data, setData] = useState<T | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    fetchData()
  }, [endpoint])

  const fetchData = async () => {
    setLoading(true)
    setError(null)
    try {
      const response = await api.get<T>(endpoint)
      if (response.isSuccess && response.data) {
        setData(response.data)
      } else {
        setError(response.errors?.[0] || 'An error occurred')
      }
    } catch (err) {
      setError('Failed to fetch data')
    } finally {
      setLoading(false)
    }
  }

  const refetch = async () => {
    await fetchData()
  }

  return { data, loading, error, refetch }
}

export function useMutation<T>(endpoint: string, method: 'POST' | 'PATCH' | 'DELETE' = 'POST') {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const mutate = async (payload?: any): Promise<ApiResponse<T>> => {
    setLoading(true)
    setError(null)
    try {
      let response: ApiResponse<T>
      if (method === 'DELETE') {
        response = await api.delete<T>(endpoint)
      } else if (method === 'PATCH') {
        response = await api.patch<T>(endpoint, payload)
      } else {
        response = await api.post<T>(endpoint, payload)
      }

      if (!response.isSuccess) {
        setError(response.errors?.[0] || 'An error occurred')
      }

      return response
    } catch (err) {
      const errorMsg = 'Failed to complete request'
      setError(errorMsg)
      return { isSuccess: false, errors: [errorMsg] }
    } finally {
      setLoading(false)
    }
  }

  return { mutate, loading, error }
}
