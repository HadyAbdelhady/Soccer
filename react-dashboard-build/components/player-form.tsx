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
import type { Player, PlayerPosition } from '@/lib/types'

interface PlayerFormProps {
  teamId?: string
  onSuccess?: (player: Player) => void
}

export function PlayerForm({ teamId, onSuccess }: PlayerFormProps) {
  const [name, setName] = useState('')
  const [shirtNumber, setShirtNumber] = useState('')
  const [position, setPosition] = useState<PlayerPosition>('MIDFIELDER')
  const [nationality, setNationality] = useState('')
  const [dateOfBirth, setDateOfBirth] = useState('')
  const [error, setError] = useState('')

  const { mutate: createPlayer, loading } = useMutation<Player>('/player', 'POST')

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError('')

    if (!name || !shirtNumber) {
      setError('Player name and shirt number are required')
      return
    }

    if (!teamId) {
      setError('Team ID is required')
      return
    }

    const response = await createPlayer({
      teamId,
      name,
      shirtNumber: parseInt(shirtNumber),
      position,
      nationality: nationality || undefined,
      dateOfBirth: dateOfBirth || undefined,
    })

    if (response.isSuccess && response.data) {
      setName('')
      setShirtNumber('')
      setPosition('MIDFIELDER')
      setNationality('')
      setDateOfBirth('')
      onSuccess?.(response.data as Player)
    } else {
      setError(response.errors?.[0] || 'Failed to add player')
    }
  }

  return (
    <Card className="border-border">
      <CardHeader>
        <CardTitle>Add Player</CardTitle>
        <CardDescription>Register a new player to the team</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-6">
          {error && (
            <Alert variant="destructive" className="mb-4">
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          <div className="space-y-2">
            <Label htmlFor="playerName" className="text-foreground">
              Player Name *
            </Label>
            <Input
              id="playerName"
              placeholder="e.g., John Smith"
              value={name}
              onChange={(e) => setName(e.target.value)}
              disabled={loading}
              className="bg-secondary border-border"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="shirtNumber" className="text-foreground">
                Shirt Number *
              </Label>
              <Input
                id="shirtNumber"
                type="number"
                placeholder="e.g., 10"
                value={shirtNumber}
                onChange={(e) => setShirtNumber(e.target.value)}
                disabled={loading}
                className="bg-secondary border-border"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="position" className="text-foreground">
                Position
              </Label>
              <Select value={position} onValueChange={(v: any) => setPosition(v)} disabled={loading}>
                <SelectTrigger className="bg-secondary border-border">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent className="bg-card border-border">
                  <SelectItem value="GOALKEEPER">Goalkeeper</SelectItem>
                  <SelectItem value="DEFENDER">Defender</SelectItem>
                  <SelectItem value="MIDFIELDER">Midfielder</SelectItem>
                  <SelectItem value="FORWARD">Forward</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="nationality" className="text-foreground">
                Nationality
              </Label>
              <Input
                id="nationality"
                placeholder="e.g., England"
                value={nationality}
                onChange={(e) => setNationality(e.target.value)}
                disabled={loading}
                className="bg-secondary border-border"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="dob" className="text-foreground">
                Date of Birth
              </Label>
              <Input
                id="dob"
                type="date"
                value={dateOfBirth}
                onChange={(e) => setDateOfBirth(e.target.value)}
                disabled={loading}
                className="bg-secondary border-border"
              />
            </div>
          </div>

          <Button
            type="submit"
            className="w-full bg-primary hover:bg-primary/90"
            disabled={loading || !name || !shirtNumber || !teamId}
          >
            {loading ? 'Adding...' : 'Add Player'}
          </Button>
        </form>
      </CardContent>
    </Card>
  )
}
