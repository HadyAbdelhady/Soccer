'use client'

import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { useMutation } from '@/lib/hooks'
import type { Tournament } from '@/lib/types'

interface TournamentFormProps {
  onSuccess?: (tournament: Tournament) => void
}

export function TournamentForm({ onSuccess }: TournamentFormProps) {
  const [name, setName] = useState('')
  const [type, setType] = useState<'SINGLE_GROUP' | 'MULTI_GROUP_KNOCKOUT'>('SINGLE_GROUP')
  const [startDate, setStartDate] = useState('')
  const [endDate, setEndDate] = useState('')
  const [error, setError] = useState('')

  const { mutate: createTournament, loading } = useMutation<Tournament>('/tournament', 'POST')

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError('')

    if (!name || !startDate || !endDate) {
      setError('Please fill in all fields')
      return
    }

    if (new Date(startDate) >= new Date(endDate)) {
      setError('End date must be after start date')
      return
    }

    const response = await createTournament({
      name,
      type,
      startDate: new Date(startDate).toISOString(),
      endDate: new Date(endDate).toISOString(),
    })

    if (response.isSuccess && response.data) {
      setName('')
      setType('SINGLE_GROUP')
      setStartDate('')
      setEndDate('')
      onSuccess?.(response.data as Tournament)
    } else {
      setError(response.errors?.[0] || 'Failed to create tournament')
    }
  }

  return (
    <Card className="border-border">
      <CardHeader>
        <CardTitle>Create New Tournament</CardTitle>
        <CardDescription>Set up a new soccer tournament</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-6">
          {error && <Alert variant="destructive" className="mb-4">
            <AlertDescription>{error}</AlertDescription>
          </Alert>}

          <div className="space-y-2">
            <Label htmlFor="name" className="text-foreground">
              Tournament Name
            </Label>
            <Input
              id="name"
              placeholder="e.g., Summer Championship 2024"
              value={name}
              onChange={(e) => setName(e.target.value)}
              disabled={loading}
              className="bg-secondary border-border"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="type" className="text-foreground">
              Tournament Type
            </Label>
            <Select value={type} onValueChange={(v: any) => setType(v)} disabled={loading}>
              <SelectTrigger className="bg-secondary border-border">
                <SelectValue />
              </SelectTrigger>
              <SelectContent className="bg-card border-border">
                <SelectItem value="SINGLE_GROUP">Single Group</SelectItem>
                <SelectItem value="MULTI_GROUP_KNOCKOUT">Multi-Group Knockout</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="startDate" className="text-foreground">
                Start Date
              </Label>
              <Input
                id="startDate"
                type="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                disabled={loading}
                className="bg-secondary border-border"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="endDate" className="text-foreground">
                End Date
              </Label>
              <Input
                id="endDate"
                type="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                disabled={loading}
                className="bg-secondary border-border"
              />
            </div>
          </div>

          <Button
            type="submit"
            className="w-full bg-primary hover:bg-primary/90"
            disabled={loading || !name || !startDate || !endDate}
          >
            {loading ? 'Creating...' : 'Create Tournament'}
          </Button>
        </form>
      </CardContent>
    </Card>
  )
}
