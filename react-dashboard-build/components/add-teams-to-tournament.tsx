'use client'

import { useEffect, useState } from 'react'
import { useRouter, useSearchParams } from 'next/navigation'
import { api } from '@/lib/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Checkbox } from '@/components/ui/checkbox'
import { Badge } from '@/components/ui/badge'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { Loader2, ArrowLeft, Users, Check } from 'lucide-react'

interface Team {
  id: string
  name: string
  username: string
}

interface Tournament {
  id: string
  name: string
  type: string
  startDate: string
  endDate: string
}

export function AddTeamsToTournament() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const tournamentId = searchParams.get('id')
  
  const [tournament, setTournament] = useState<Tournament | null>(null)
  const [availableTeams, setAvailableTeams] = useState<Team[]>([])
  const [selectedTeams, setSelectedTeams] = useState<string[]>([])
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!tournamentId) {
      router.push('/dashboard/tournaments')
      return
    }
    
    loadData()
  }, [tournamentId])

  const loadData = async () => {
    setLoading(true)
    setError(null)
    try {
      // Load tournament details using the new endpoint
      const resolvedParams = await searchParams
      const tournamentResponse = await api.get<any>(`/tournament/${resolvedParams.get('id')}`)
      if (tournamentResponse.isSuccess && tournamentResponse.data) {
        setTournament(tournamentResponse.data)
      } else {
        setError('Failed to load tournament details')
      }

      // Load available teams
      const teamsResponse = await api.getTeamsNotInTournaments()
      if (teamsResponse.isSuccess && teamsResponse.data) {
        // Backend returns { teams: [...] } structure
        const teamsData = teamsResponse.data as any
        setAvailableTeams(teamsData.teams || [])
      } else {
        setError('Failed to load available teams')
      }
    } catch (error) {
      setError('Failed to load data')
      console.error('Error loading data:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleTeamToggle = (teamId: string, checked: boolean) => {
    if (checked) {
      setSelectedTeams([...selectedTeams, teamId])
    } else {
      setSelectedTeams(selectedTeams.filter(id => id !== teamId))
    }
  }

  const handleSubmit = async () => {
    if (selectedTeams.length === 0) {
      setError('Please select at least one team')
      return
    }

    setSubmitting(true)
    setError('')

    try {
      const response = await api.addTeamsToTournament(tournamentId!, selectedTeams)
      if (response.isSuccess) {
        router.push('/dashboard/tournaments')
      } else {
        setError(response.errors?.[0] || 'Failed to add teams to tournament')
      }
    } catch (error) {
      console.error('Error adding teams:', error)
      setError('Failed to add teams to tournament')
    } finally {
      setSubmitting(false)
    }
  }

  const handleBack = () => {
    router.push('/dashboard/tournaments')
  }

  const getTypeLabel = (type: string) => {
    switch (type) {
      case 'SINGLE_GROUP':
        return 'Single Group'
      case 'MULTI_GROUP_KNOCKOUT':
        return 'Multi-Group Knockout'
      default:
        return type
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
        <span className="ml-2 text-muted-foreground">Loading data...</span>
      </div>
    )
  }

  if (!tournament) {
    return (
      <div className="text-center py-12">
        <h3 className="text-lg font-semibold text-foreground mb-2">Tournament not found</h3>
        <Button onClick={handleBack} variant="outline">
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to Tournaments
        </Button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button onClick={handleBack} variant="outline" size="sm">
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back
        </Button>
        <div>
          <h1 className="text-3xl font-bold text-foreground">Add Teams to Tournament</h1>
          <p className="text-muted-foreground mt-1">Select teams to add to {tournament.name}</p>
        </div>
      </div>

      {/* Tournament Info */}
      <Card className="border-border">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Users className="h-5 w-5" />
            {tournament.name}
          </CardTitle>
          <CardDescription>
            {getTypeLabel(tournament.type)}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 gap-4 text-sm">
            <div>
              <p className="text-muted-foreground">Start Date</p>
              <p className="text-foreground font-medium">{new Date(tournament.startDate).toLocaleDateString()}</p>
            </div>
            <div>
              <p className="text-muted-foreground">End Date</p>
              <p className="text-foreground font-medium">{new Date(tournament.endDate).toLocaleDateString()}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Error Alert */}
      {error && (
        <Alert variant="destructive">
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {/* Teams Selection */}
      <Card className="border-border">
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle>Select Teams</CardTitle>
              <CardDescription>
                {availableTeams.length} teams available for selection
              </CardDescription>
            </div>
            {selectedTeams.length > 0 && (
              <Badge variant="secondary">
                {selectedTeams.length} team{selectedTeams.length !== 1 ? 's' : ''} selected
              </Badge>
            )}
          </div>
        </CardHeader>
        <CardContent>
          {availableTeams.length === 0 ? (
            <div className="text-center py-8">
              <Users className="h-12 w-12 mx-auto text-muted-foreground mb-4" />
              <h3 className="text-lg font-semibold text-foreground mb-2">No teams available</h3>
              <p className="text-muted-foreground mb-4">All teams are already in this tournament</p>
              <Button onClick={() => router.push('/dashboard/teams')}>
                Create New Team
              </Button>
            </div>
          ) : (
            <div className="space-y-4">
              <div className="grid gap-3">
                {availableTeams.map((team) => (
                  <div
                    key={team.id}
                    className="flex items-center space-x-3 p-3 rounded-lg border border-border hover:bg-secondary/50 transition-colors"
                  >
                    <Checkbox
                      id={team.id}
                      checked={selectedTeams.includes(team.id)}
                      onCheckedChange={(checked) => handleTeamToggle(team.id, checked as boolean)}
                    />
                    <div className="flex-1">
                      <label
                        htmlFor={team.id}
                        className="text-sm font-medium text-foreground cursor-pointer"
                      >
                        {team.name}
                      </label>
                      <p className="text-xs text-muted-foreground">@{team.username}</p>
                    </div>
                    {selectedTeams.includes(team.id) && (
                      <Check className="h-4 w-4 text-green-500" />
                    )}
                  </div>
                ))}
              </div>

              {/* Action Buttons */}
              <div className="flex items-center gap-3 pt-4 border-t border-border">
                <Button
                  onClick={handleSubmit}
                  disabled={selectedTeams.length === 0 || submitting}
                  className="bg-primary hover:bg-primary/90"
                >
                  {submitting ? (
                    <>
                      <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                      Adding Teams...
                    </>
                  ) : (
                    <>
                      <Users className="h-4 w-4 mr-2" />
                      Add {selectedTeams.length} Team{selectedTeams.length !== 1 ? 's' : ''}
                    </>
                  )}
                </Button>
                <Button onClick={handleBack} variant="outline">
                  Cancel
                </Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
