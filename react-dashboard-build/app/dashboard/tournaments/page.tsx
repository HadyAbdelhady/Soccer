'use client'

import { useEffect, useState } from 'react'
import { api } from '@/lib/api'
import type { Tournament, Group } from '@/lib/types'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog'
import { TournamentForm } from '@/components/tournament-form'
import { AlertTriangle } from 'lucide-react'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'

export default function TournamentsPage() {
  const [tournaments, setTournaments] = useState<Tournament[]>([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [resetDialogOpen, setResetDialogOpen] = useState(false)
  const [selectedTournament, setSelectedTournament] = useState<Tournament | null>(null)
  const [resetting, setResetting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [generateGroupsDialogOpen, setGenerateGroupsDialogOpen] = useState(false)
  const [generateMatchesDialogOpen, setGenerateMatchesDialogOpen] = useState(false)
  const [generatingGroups, setGeneratingGroups] = useState(false)
  const [generatingMatches, setGeneratingMatches] = useState(false)
  const [generateError, setGenerateError] = useState<string | null>(null)
  const [selectedTournamentForDetails, setSelectedTournamentForDetails] = useState<Tournament | null>(null)
  const [tournamentGroups, setTournamentGroups] = useState<Group[]>([])
  const [tournamentTeams, setTournamentTeams] = useState<any[]>([])
  const [tournamentMatches, setTournamentMatches] = useState<any[]>([])

  useEffect(() => {
    fetchTournaments()
  }, [])

  const fetchTournaments = async () => {
    setLoading(true)
    try {
      const response = await api.getTournamentsWithTeamCount()
      if (response.isSuccess && response.data) {
        // Backend returns { tournaments: [] } with teamCount
        const tournamentsData = response.data.tournaments || []
        const mappedTournaments: Tournament[] = tournamentsData.map((t: any) => ({
          id: t.id,
          name: t.name,
          type: t.type,
          startDate: t.startDate,
          endDate: t.endDate,
          teamCount: t.teamCount || 0,
          // Add placeholder values for fields not in backend response
          status: 'UPCOMING',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }))
        setTournaments(mappedTournaments)
      }
    } catch (error) {
      console.error('Error fetching tournaments:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleTournamentCreated = (newTournament: Tournament) => {
    setTournaments([...tournaments, newTournament])
    setShowForm(false)
  }

  const handleResetSchedule = async () => {
    if (!selectedTournament) return
    
    setResetting(true)
    setError(null)
    try {
      const response = await api.resetSchedule(selectedTournament.id)
      if (response.isSuccess) {
        // Refresh tournaments list to show updated state
        await fetchTournaments()
        setResetDialogOpen(false)
        setSelectedTournament(null)
      } else {
        const errorMessage = response.errors?.[0] || 'Failed to reset schedule'
        setError(errorMessage)
        console.error('Failed to reset schedule:', response.errors)
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'An unexpected error occurred'
      setError(errorMessage)
      console.error('Error resetting schedule:', error)
    } finally {
      setResetting(false)
    }
  }

  const openResetDialog = (tournament: Tournament) => {
    setSelectedTournament(tournament)
    setResetDialogOpen(true)
  }

  const openGenerateGroupsDialog = (tournament: Tournament) => {
    setSelectedTournament(tournament)
    setGenerateGroupsDialogOpen(true)
    setGenerateError(null)
  }

  const openGenerateMatchesDialog = (tournament: Tournament) => {
    setSelectedTournament(tournament)
    setGenerateMatchesDialogOpen(true)
    setGenerateError(null)
  }

  const handleGenerateGroups = async () => {
    if (!selectedTournament) return
    
    setGeneratingGroups(true)
    setGenerateError(null)
    try {
      const response = await api.generateGroups(selectedTournament.id)
      if (response.isSuccess) {
        await fetchTournaments()
        setGenerateGroupsDialogOpen(false)
        setSelectedTournament(null)
      } else {
        const errorMessage = response.errors?.[0] || 'Failed to generate groups'
        setGenerateError(errorMessage)
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'An unexpected error occurred'
      setGenerateError(errorMessage)
    } finally {
      setGeneratingGroups(false)
    }
  }

  const handleGenerateMatches = async () => {
    if (!selectedTournament) return
    
    setGeneratingMatches(true)
    setGenerateError(null)
    try {
      const response = await api.generateMatches(selectedTournament.id)
      if (response.isSuccess) {
        await fetchTournaments()
        setGenerateMatchesDialogOpen(false)
        setSelectedTournament(null)
      } else {
        const errorMessage = response.errors?.[0] || 'Failed to generate matches'
        setGenerateError(errorMessage)
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'An unexpected error occurred'
      setGenerateError(errorMessage)
    } finally {
      setGeneratingMatches(false)
    }
  }

  const fetchTournamentDetails = async (tournamentId: string) => {
    try {
      console.log('Fetching tournament details for ID:', tournamentId)
      const [groupsResponse, teamsResponse, matchesResponse] = await Promise.all([
        api.get<Group[]>(`/group/tournament/${tournamentId}`), // Correct endpoint
        api.get<any[]>(`/team`), // Get all teams (no tournament filter)
        api.get<any[]>(`/match/getAllMatches?tournamentId=${tournamentId}`) // Correct endpoint
      ])

      console.log('API Responses:')
      console.log('Groups response:', groupsResponse)
      console.log('Teams response:', teamsResponse)
      console.log('Matches response:', matchesResponse)

      if (groupsResponse.isSuccess) {
        console.log('Setting groups:', groupsResponse.data)
        // Handle the correct response structure
        const groupsData = groupsResponse.data as any
        if (groupsData && groupsData.groups) {
          setTournamentGroups(groupsData.groups)
        } else {
          console.error('Unexpected groups response structure:', groupsData)
          setTournamentGroups([])
        }
      } else {
        console.error('Groups API failed:', groupsResponse.errors)
        setTournamentGroups([])
      }
      
      if (teamsResponse.isSuccess) {
        console.log('Setting teams:', teamsResponse.data)
        // Teams API returns GetAllTeamsResponse with Teams property
        const teamsData = teamsResponse.data as any
        if (teamsData && teamsData.teams) {
          setTournamentTeams(teamsData.teams)
        } else {
          console.error('Unexpected teams response structure:', teamsData)
          setTournamentTeams([])
        }
      } else {
        console.error('Teams API failed:', teamsResponse.errors)
        setTournamentTeams([]) // Set empty array on error
      }
      
      if (matchesResponse.isSuccess) {
        console.log('Setting matches:', matchesResponse.data)
        // Handle GenerateTournamentMatchesResponse structure
        const matchesData = matchesResponse.data as any
        if (matchesData && matchesData.matches) {
          setTournamentMatches(matchesData.matches)
        } else if (matchesData && matchesData.tournaments) {
          // Handle GetAllMatchesResponse structure if needed
          const tournament = matchesData.tournaments.find((t: any) => t.id === selectedTournamentForDetails?.id)
          setTournamentMatches(tournament?.matches || [])
        } else {
          console.error('Unexpected matches response structure:', matchesData)
          setTournamentMatches([])
        }
      } else {
        console.error('Matches API failed:', matchesResponse.errors)
        setTournamentMatches([]) // Set empty array on error
      }
    } catch (error) {
      console.error('Error fetching tournament details:', error)
      setTournamentGroups([])
      setTournamentTeams([])
      setTournamentMatches([])
    }
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'UPCOMING':
        return 'bg-blue-500/20 text-blue-400'
      case 'ONGOING':
        return 'bg-green-500/20 text-green-400'
      case 'COMPLETED':
        return 'bg-gray-500/20 text-gray-400'
      default:
        return 'bg-gray-500/20 text-gray-400'
    }
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

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Tournaments</h1>
          <p className="text-muted-foreground mt-1">Create and manage soccer tournaments</p>
        </div>
        <Dialog open={showForm} onOpenChange={setShowForm}>
          <DialogTrigger asChild>
            <Button className="bg-primary hover:bg-primary/90">+ Create Tournament</Button>
          </DialogTrigger>
          <DialogContent className="max-w-md border-border bg-card">
            <DialogHeader>
              <DialogTitle>Create Tournament</DialogTitle>
              <DialogDescription>Add a new soccer tournament to the system</DialogDescription>
            </DialogHeader>
            <TournamentForm onSuccess={handleTournamentCreated} />
          </DialogContent>
        </Dialog>
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-12">
          <div className="text-muted-foreground">Loading tournaments...</div>
        </div>
      ) : tournaments.length === 0 ? (
        <Card className="border-border">
          <CardContent className="py-12 text-center">
            <div className="text-4xl mb-4">🏆</div>
            <h3 className="text-xl font-semibold text-foreground mb-2">No tournaments yet</h3>
            <p className="text-muted-foreground mb-4">Create your first tournament to get started</p>
            <Button className="bg-primary hover:bg-primary/90">Create Tournament</Button>
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-4">
          {tournaments.map((tournament) => (
            <Card key={tournament.id} className="border-border hover:bg-secondary/50 transition-colors cursor-pointer">
              <CardHeader>
                <div className="flex items-start justify-between">
                  <div className="flex-1">
                    <CardTitle className="text-xl text-foreground">{tournament.name}</CardTitle>
                    <CardDescription className="mt-2">
                      {getTypeLabel(tournament.type)}
                    </CardDescription>
                  </div>
                  <Badge className={getStatusColor(tournament.status)}>{tournament.status}</Badge>
                </div>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-3 gap-4 text-sm">
                  <div>
                    <p className="text-muted-foreground">Start Date</p>
                    <p className="text-foreground font-medium">{new Date(tournament.startDate).toLocaleDateString()}</p>
                  </div>
                  <div>
                    <p className="text-muted-foreground">End Date</p>
                    <p className="text-foreground font-medium">{new Date(tournament.endDate).toLocaleDateString()}</p>
                  </div>
                  <div className="text-right space-x-2">
                    <Button 
                      variant="outline" 
                      className="border-border text-primary hover:bg-secondary"
                      onClick={() => window.location.href = `/dashboard/tournaments/${tournament.id}`}
                    >
                      View Details
                    </Button>
                    <Button 
                      variant="outline" 
                      className="border-border text-primary hover:bg-secondary"
                      onClick={() => window.location.href = `/dashboard/tournaments/add-teams?id=${tournament.id}`}
                    >
                      Add Teams
                    </Button>
                    <Button 
                      variant="outline" 
                      className="border-border text-blue-600 hover:bg-blue-50"
                      onClick={() => openGenerateGroupsDialog(tournament)}
                      disabled={!tournament.teamCount || tournament.teamCount === 0}
                      title={!tournament.teamCount || tournament.teamCount === 0 ? 
                        "Add teams to tournament before generating groups" : 
                        "Generate tournament groups"}
                    >
                      Generate Groups
                    </Button>
                    <Button 
                      variant="outline" 
                      className="border-border text-green-600 hover:bg-green-50"
                      onClick={() => openGenerateMatchesDialog(tournament)}
                      disabled={!tournament.teamCount || tournament.teamCount === 0}
                      title={!tournament.teamCount || tournament.teamCount === 0 ? 
                        "Add teams to tournament before generating matches" : 
                        "Generate tournament matches"}
                    >
                      Generate Matches
                    </Button>
                    <Button 
                      variant="outline" 
                      className="border-border text-orange-600 hover:bg-orange-50"
                      onClick={() => openResetDialog(tournament)}
                      disabled={!tournament.teamCount || tournament.teamCount === 0}
                      title={!tournament.teamCount || tournament.teamCount === 0 ? 
                        "Add teams to tournament before resetting schedule" : 
                        "Reset tournament schedule"}
                    >
                      Reset Schedule
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {/* Tournament Details Section */}
      {selectedTournamentForDetails && (
        <div id="tournament-details-section" className="mt-8 border-t pt-8">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h2 className="text-2xl font-bold text-foreground">
                {selectedTournamentForDetails.name}
              </h2>
              <div className="flex gap-2 mt-2">
                <Badge className={getStatusColor(selectedTournamentForDetails.status)}>
                  {selectedTournamentForDetails.status}
                </Badge>
                <Badge variant="secondary">{selectedTournamentForDetails.type}</Badge>
              </div>
            </div>
            <Button 
              variant="outline" 
              onClick={() => setSelectedTournamentForDetails(null)}
              className="border-border text-primary hover:bg-secondary"
            >
              Close Details
            </Button>
          </div>

          {/* Tournament Details Tabs */}
          <Tabs defaultValue="groups" className="w-full">
            <TabsList className="grid w-full grid-cols-3 bg-secondary">
              <TabsTrigger value="groups">Groups</TabsTrigger>
              <TabsTrigger value="teams">Teams</TabsTrigger>
              <TabsTrigger value="matches">Matches</TabsTrigger>
            </TabsList>

            <TabsContent value="groups" className="space-y-4 mt-6">
              {tournamentGroups.length === 0 ? (
                <Card className="border-border">
                  <CardContent className="py-12 text-center">
                    <p className="text-muted-foreground">No groups created yet</p>
                    <Button 
                      className="mt-4 bg-primary hover:bg-primary/90"
                      onClick={() => {
                        setSelectedTournament(selectedTournamentForDetails)
                        setGenerateGroupsDialogOpen(true)
                      }}
                    >
                      Create Groups
                    </Button>
                  </CardContent>
                </Card>
              ) : (
                <div className="grid gap-4">
                  {tournamentGroups.map((group) => (
                    <Card key={group.id} className="border-border hover:bg-secondary/50 transition-colors">
                      <CardHeader>
                        <CardTitle>{group.name}</CardTitle>
                      </CardHeader>
                      <CardContent>
                        <p className="text-sm text-muted-foreground">Stage: {group.stage}</p>
                      </CardContent>
                    </Card>
                  ))}
                </div>
              )}
            </TabsContent>

            <TabsContent value="teams" className="space-y-4 mt-6">
              {tournamentTeams.length === 0 ? (
                <Card className="border-border">
                  <CardContent className="py-12 text-center">
                    <p className="text-muted-foreground">No teams in this tournament yet</p>
                    <Button 
                      className="mt-4 bg-primary hover:bg-primary/90"
                      onClick={() => window.location.href = `/dashboard/tournaments/add-teams?id=${selectedTournamentForDetails.id}`}
                    >
                      Add Teams
                    </Button>
                  </CardContent>
                </Card>
              ) : (
                <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                  {tournamentTeams.map((team) => (
                    <Card key={team.id} className="border-border hover:bg-secondary/50 transition-colors">
                      <CardHeader>
                        <CardTitle className="text-lg">{team.name}</CardTitle>
                      </CardHeader>
                      <CardContent>
                        <p className="text-sm text-muted-foreground">Type: {team.type}</p>
                        <p className="text-sm text-muted-foreground">Players: {team.playerCount || 0}</p>
                      </CardContent>
                    </Card>
                  ))}
                </div>
              )}
            </TabsContent>

            <TabsContent value="matches" className="space-y-4 mt-6">
              {tournamentMatches.length === 0 ? (
                <Card className="border-border">
                  <CardContent className="py-12 text-center">
                    <p className="text-muted-foreground">No matches scheduled yet</p>
                    <Button 
                      className="mt-4 bg-primary hover:bg-primary/90"
                      onClick={() => {
                        setSelectedTournament(selectedTournamentForDetails)
                        setGenerateMatchesDialogOpen(true)
                      }}
                    >
                      Generate Matches
                    </Button>
                  </CardContent>
                </Card>
              ) : (
                <div className="space-y-4">
                  {tournamentMatches.map((match) => (
                    <Card key={match.id} className="border-border hover:bg-secondary/50 transition-colors">
                      <CardContent className="p-4">
                        <div className="flex items-center justify-between">
                          <div className="flex items-center space-x-4">
                            <div className="text-center">
                              <p className="font-semibold">{match.homeTeam?.name || 'TBD'}</p>
                            </div>
                            <div className="text-center">
                              <p className="text-2xl font-bold">VS</p>
                            </div>
                            <div className="text-center">
                              <p className="font-semibold">{match.awayTeam?.name || 'TBD'}</p>
                            </div>
                          </div>
                          <div className="text-right">
                            <p className="text-sm text-muted-foreground">
                              {new Date(match.date).toLocaleDateString()}
                            </p>
                            <Badge variant="secondary">
                              {match.status || 'Scheduled'}
                            </Badge>
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  ))}
                </div>
              )}
            </TabsContent>
          </Tabs>
        </div>
      )}

      {/* Reset Schedule Confirmation Dialog */}
      <Dialog open={resetDialogOpen} onOpenChange={setResetDialogOpen}>
        <DialogContent className="border-border bg-card">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2 text-foreground">
              <AlertTriangle className="h-5 w-5 text-orange-500" />
              Reset Tournament Schedule
            </DialogTitle>
            <DialogDescription className="text-muted-foreground">
              Are you sure you want to reset the schedule for "{selectedTournament?.name}"? 
              This will regenerate all groups and matches, removing any existing match data.
            </DialogDescription>
          </DialogHeader>
          
          {/* Error Display */}
          {error && (
            <div className="bg-red-50 border border-red-200 rounded-md p-3 mb-4">
              <div className="flex items-center gap-2 text-red-800">
                <AlertTriangle className="h-4 w-4" />
                <span className="text-sm font-medium">{error}</span>
              </div>
              <p className="text-xs text-red-600 mt-1">
                Please add teams to this tournament before resetting schedule.
              </p>
            </div>
          )}
          
          <div className="flex justify-end gap-3 mt-6">
            <Button 
              variant="outline" 
              onClick={() => {
                setResetDialogOpen(false)
                setSelectedTournament(null)
                setError(null)
              }}
              disabled={resetting}
            >
              Cancel
            </Button>
            <Button 
              onClick={handleResetSchedule}
              disabled={resetting}
              className="bg-orange-600 hover:bg-orange-700 text-white"
            >
              {resetting ? 'Resetting...' : 'Reset Schedule'}
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Generate Groups Confirmation Dialog */}
      <Dialog open={generateGroupsDialogOpen} onOpenChange={setGenerateGroupsDialogOpen}>
        <DialogContent className="border-border bg-card">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2 text-foreground">
              <AlertTriangle className="h-5 w-5 text-blue-500" />
              Generate Tournament Groups
            </DialogTitle>
            <DialogDescription className="text-muted-foreground">
              Are you sure you want to generate groups for "{selectedTournament?.name}"? 
              This will create tournament groups and assign teams to them.
            </DialogDescription>
          </DialogHeader>
          
          {/* Error Display */}
          {generateError && (
            <div className="bg-red-50 border border-red-200 rounded-md p-3 mb-4">
              <div className="flex items-center gap-2 text-red-800">
                <AlertTriangle className="h-4 w-4" />
                <span className="text-sm font-medium">{generateError}</span>
              </div>
            </div>
          )}
          
          <div className="flex justify-end gap-3 mt-6">
            <Button 
              variant="outline" 
              onClick={() => {
                setGenerateGroupsDialogOpen(false)
                setSelectedTournament(null)
                setGenerateError(null)
              }}
              disabled={generatingGroups}
            >
              Cancel
            </Button>
            <Button 
              onClick={handleGenerateGroups}
              disabled={generatingGroups}
              className="bg-blue-600 hover:bg-blue-700 text-white"
            >
              {generatingGroups ? 'Generating...' : 'Generate Groups'}
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Generate Matches Confirmation Dialog */}
      <Dialog open={generateMatchesDialogOpen} onOpenChange={setGenerateMatchesDialogOpen}>
        <DialogContent className="border-border bg-card">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2 text-foreground">
              <AlertTriangle className="h-5 w-5 text-green-500" />
              Generate Tournament Matches
            </DialogTitle>
            <DialogDescription className="text-muted-foreground">
              Are you sure you want to generate matches for "{selectedTournament?.name}"? 
              This will create match fixtures based on the tournament groups.
            </DialogDescription>
          </DialogHeader>
          
          {/* Error Display */}
          {generateError && (
            <div className="bg-red-50 border border-red-200 rounded-md p-3 mb-4">
              <div className="flex items-center gap-2 text-red-800">
                <AlertTriangle className="h-4 w-4" />
                <span className="text-sm font-medium">{generateError}</span>
              </div>
            </div>
          )}
          
          <div className="flex justify-end gap-3 mt-6">
            <Button 
              variant="outline" 
              onClick={() => {
                setGenerateMatchesDialogOpen(false)
                setSelectedTournament(null)
                setGenerateError(null)
              }}
              disabled={generatingMatches}
            >
              Cancel
            </Button>
            <Button 
              onClick={handleGenerateMatches}
              disabled={generatingMatches}
              className="bg-green-600 hover:bg-green-700 text-white"
            >
              {generatingMatches ? 'Generating...' : 'Generate Matches'}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  )
}
