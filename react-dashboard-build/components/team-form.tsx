'use client'

import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { useMutation } from '@/lib/hooks'
import type { Team } from '@/lib/types'

interface TeamFormProps {
  onSuccess?: (team: Team) => void
}

export function TeamForm({ onSuccess }: TeamFormProps) {
  const [name, setName] = useState('')
  const [country, setCountry] = useState('')
  const [founded, setFounded] = useState('')
  const [error, setError] = useState('')

  const { mutate: createTeam, loading } = useMutation<Team>('/team', 'POST')

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError('')

    if (!name) {
      setError('Team name is required')
      return
    }

    const response = await createTeam({
      name,
      country: country || undefined,
      founded: founded ? parseInt(founded) : undefined,
    })

    if (response.isSuccess && response.data) {
      setName('')
      setCountry('')
      setFounded('')
      onSuccess?.(response.data as Team)
    } else {
      setError(response.errors?.[0] || 'Failed to create team')
    }
  }

  return (
    <Card className="border-border">
      <CardHeader>
        <CardTitle>Register New Team</CardTitle>
        <CardDescription>Add a new team to the system</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-6">
          {error && (
            <Alert variant="destructive" className="mb-4">
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          <div className="space-y-2">
            <Label htmlFor="teamName" className="text-foreground">
              Team Name *
            </Label>
            <Input
              id="teamName"
              placeholder="e.g., Manchester United"
              value={name}
              onChange={(e) => setName(e.target.value)}
              disabled={loading}
              className="bg-secondary border-border"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="country" className="text-foreground">
              Country
            </Label>
            <Input
              id="country"
              placeholder="e.g., England"
              value={country}
              onChange={(e) => setCountry(e.target.value)}
              disabled={loading}
              className="bg-secondary border-border"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="founded" className="text-foreground">
              Founded Year
            </Label>
            <Input
              id="founded"
              type="number"
              placeholder="e.g., 1878"
              value={founded}
              onChange={(e) => setFounded(e.target.value)}
              disabled={loading}
              className="bg-secondary border-border"
            />
          </div>

          <Button
            type="submit"
            className="w-full bg-primary hover:bg-primary/90"
            disabled={loading || !name}
          >
            {loading ? 'Creating...' : 'Register Team'}
          </Button>
        </form>
      </CardContent>
    </Card>
  )
}
