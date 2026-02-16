'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { api } from '@/lib/api'
import type { Tournament, Group, GroupStanding } from '@/lib/types'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { GroupStandings } from '@/components/group-standings'
import { ArrowLeft, Trophy } from 'lucide-react'

interface TournamentDetailPageProps {
  params: {
    id: string
  }
}

export default function TournamentDetailPage({ params }: TournamentDetailPageProps) {
  const router = useRouter()
  const [tournament, setTournament] = useState<Tournament | null>(null)
  const [groups, setGroups] = useState<Group[]>([])
  const [teams, setTeams] = useState<any[]>([])
  const [matches, setMatches] = useState<any[]>([])
  const [groupStandings, setGroupStandings] = useState<{ [key: string]: GroupStanding[] }>({})
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const fetchTournamentData = async () => {
      const resolvedParams = await params
      if (resolvedParams.id) {
        fetchTournament(resolvedParams.id)
      }
    }
    fetchTournamentData()
  }, [params])

  const fetchTournament = async (id: string) => {
    setLoading(true)
    setError(null)
    try {
      console.log('Fetching tournament with ID:', id)
      const response = await api.get<any>(`/tournament/${id}`)
      console.log('API Response:', response)
      
      // Handle different response structures
      let tournamentData = null
      if (response.isSuccess && response.data) {
        tournamentData = response.data
      }
      // Check if data is nested differently (some APIs might return data directly)
      else if (response.isSuccess && (response as any).value) {
        tournamentData = (response as any).value
      }
      
      if (tournamentData) {
        console.log('Tournament data:', tournamentData)
        setTournament({
          id: tournamentData.id,
          name: tournamentData.name,
          type: tournamentData.type,
          startDate: tournamentData.startDate,
          endDate: tournamentData.endDate,
          status: 'UPCOMING', // Default status since backend doesn't provide it
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        })
        fetchGroups()
        fetchTeams()
        fetchMatches()
      } else {
        const errorMessage = response.errors && Array.isArray(response.errors) 
          ? response.errors.join(', ') 
          : response.message || 'Failed to load tournament'
        setError(errorMessage)
        console.error('No tournament data found in response')
        console.error('Full response:', response)
        console.error('Response isSuccess:', response.isSuccess)
        console.error('Response data:', response.data)
        console.error('Response errors:', JSON.stringify(response.errors, null, 2))
        console.error('Response message:', response.message)
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to load tournament'
      setError(errorMessage)
      console.error('Error fetching tournament:', error)
    } finally {
      setLoading(false)
    }
  }

  const fetchGroups = async () => {
    try {
      const resolvedParams = await params
      console.log('Fetching groups for tournament ID:', resolvedParams.id)
      const response = await api.get<any>(`/group/tournament/${resolvedParams.id}`)
      console.log('Groups API response:', response)
      if (response.isSuccess && response.data) {
        // The backend returns TournamentGroupsResponseDto which has a Groups property
        const groupsData = response.data.groups || response.data
        console.log('Groups data received:', groupsData)
        setGroups(Array.isArray(groupsData) ? groupsData : [])
        
        // Fetch standings for each group
        if (Array.isArray(groupsData)) {
          const standingsPromises = groupsData.map((group: Group) => fetchGroupStandings(group.id))
          await Promise.all(standingsPromises)
        }
      } else {
        console.error('Groups API failed:', response.errors)
        console.error('Groups API response structure:', response)
        setGroups([])
      }
    } catch (error) {
      console.error('Error fetching groups:', error)
      setGroups([])
    }
  }

  const fetchGroupStandings = async (groupId: string) => {
    try {
      console.log('Fetching standings for group ID:', groupId)
      const response = await api.get<GroupStanding[]>(`/group/${groupId}/standings`)
      console.log('Standings API response:', response)
      if (response.isSuccess && response.data) {
        setGroupStandings(prev => ({
          ...prev,
          [groupId]: Array.isArray(response.data) ? response.data : []
        }))
      } else {
        console.error('Standings API failed:', response.errors)
        setGroupStandings(prev => ({
          ...prev,
          [groupId]: []
        }))
      }
    } catch (error) {
      console.error('Error fetching group standings:', error)
      setGroupStandings(prev => ({
        ...prev,
        [groupId]: []
      }))
    }
  }

  const fetchTeams = async () => {
    try {
      const resolvedParams = await params
      const response = await api.get<any[]>(`/team`)
      if (response.isSuccess && response.data) {
        setTeams(Array.isArray(response.data) ? response.data : [])
      } else {
        console.error('Teams API failed:', response.errors)
        setTeams([])
      }
    } catch (error) {
      console.error('Error fetching teams:', error)
      setTeams([])
    }
  }

  const fetchMatches = async () => {
    try {
      const resolvedParams = await params
      const response = await api.get<any[]>(`/match/getAllMatches?tournamentId=${resolvedParams.id}`)
      if (response.isSuccess && response.data) {
        setMatches(Array.isArray(response.data) ? response.data : [])
      } else {
        console.error('Matches API failed:', response.errors)
        setMatches([])
      }
    } catch (error) {
      console.error('Error fetching matches:', error)
      setMatches([])
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

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="text-muted-foreground">Loading tournament...</div>
      </div>
    )
  }

  if (!tournament) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="text-center max-w-md">
          <div className="text-red-500 mb-4">
            <Trophy className="h-12 w-12 mx-auto" />
          </div>
          <h2 className="text-xl font-semibold text-foreground mb-2">
            {error || 'Tournament not found'}
          </h2>
          <p className="text-muted-foreground mb-4">
            {error || 'The tournament you\'re looking for doesn\'t exist or has been removed.'}
          </p>
          <Button onClick={() => router.push('/dashboard/tournaments')}>
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back to Tournaments
          </Button>
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header with Back Button */}
      <div className="flex items-center gap-4">
        <Button 
          variant="outline" 
          onClick={() => router.push('/dashboard/tournaments')}
          className="border-border text-primary hover:bg-secondary"
        >
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back
        </Button>
        <div className="flex-1">
          <h1 className="text-3xl font-bold text-foreground">{tournament.name}</h1>
          <div className="flex gap-2 mt-2">
            <Badge className={getStatusColor(tournament.status)}>{tournament.status}</Badge>
            <Badge variant="secondary">{getTypeLabel(tournament.type)}</Badge>
          </div>
        </div>
        <Button variant="outline" className="border-border text-primary hover:bg-secondary">
          Edit Tournament
        </Button>
      </div>

      {/* Tournament Info */}
      <div className="grid grid-cols-3 gap-4">
        <Card className="border-border">
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Start Date</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-primary">
              {new Date(tournament.startDate).toLocaleDateString()}
            </div>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">End Date</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-primary">
              {new Date(tournament.endDate).toLocaleDateString()}
            </div>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Groups</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-primary">{groups.length}</div>
          </CardContent>
        </Card>
      </div>

      {/* Tabs */}
      <Tabs defaultValue="groups" className="w-full">
        <TabsList className="grid w-full grid-cols-3 bg-secondary">
          <TabsTrigger value="groups">Groups</TabsTrigger>
          <TabsTrigger value="teams">Teams</TabsTrigger>
          <TabsTrigger value="matches">Matches</TabsTrigger>
        </TabsList>

        <TabsContent value="groups" className="space-y-4 mt-6">
          {groups.length === 0 ? (
            <Card className="border-border">
              <CardContent className="py-12 text-center">
                <p className="text-muted-foreground">No groups created yet</p>
                <Button className="mt-4 bg-primary hover:bg-primary/90">Create Groups</Button>
              </CardContent>
            </Card>
          ) : (
            <div className="space-y-6">
              {groups.map((group) => (
                <GroupStandings
                  key={group.id}
                  groupName={group.name}
                  standings={groupStandings[group.id] || []}
                />
              ))}
            </div>
          )}
        </TabsContent>

        <TabsContent value="teams" className="space-y-4 mt-6">
          {teams.length === 0 ? (
            <Card className="border-border">
              <CardContent className="py-12 text-center">
                <p className="text-muted-foreground">No teams in this tournament yet</p>
                <Button 
                  className="mt-4 bg-primary hover:bg-primary/90"
                  onClick={() => window.location.href = `/dashboard/tournaments/add-teams?id=${params.id}`}
                >
                  Add Teams
                </Button>
              </CardContent>
            </Card>
          ) : (
            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
              {teams.map((team) => (
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
          {matches.length === 0 ? (
            <Card className="border-border">
              <CardContent className="py-12 text-center">
                <p className="text-muted-foreground">No matches scheduled yet</p>
                <Button className="mt-4 bg-primary hover:bg-primary/90">Generate Matches</Button>
              </CardContent>
            </Card>
          ) : (
            <div className="space-y-4">
              {matches.map((match) => (
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
  )
}
