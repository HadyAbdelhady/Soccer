'use client'

import { useEffect, useState } from 'react'
import { api } from '@/lib/api'
import type { Group, GroupStanding, Match } from '@/lib/types'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { GroupStandings } from '@/components/group-standings'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'

interface GroupDetailPageProps {
  params: {
    id: string
  }
}

export default function GroupDetailPage({ params }: GroupDetailPageProps) {
  const [group, setGroup] = useState<Group | null>(null)
  const [standings, setStandings] = useState<GroupStanding[]>([])
  const [matches, setMatches] = useState<Match[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchGroupDetails()
  }, [params.id])

  const fetchGroupDetails = async () => {
    setLoading(true)
    try {
      const groupResponse = await api.get<Group>(`/group/${params.id}`)
      if (groupResponse.isSuccess && groupResponse.data) {
        setGroup(groupResponse.data)

        // Fetch standings
        try {
          const standingsResponse = await api.get<GroupStanding[]>(`/group/${params.id}/standings`)
          if (standingsResponse.isSuccess && standingsResponse.data) {
            setStandings(standingsResponse.data)
          }
        } catch (error) {
          console.error('Error fetching standings:', error)
        }

        // Fetch matches
        try {
          const matchesResponse = await api.get<Match[]>(`/group/${params.id}/matches`)
          if (matchesResponse.isSuccess && matchesResponse.data) {
            setMatches(matchesResponse.data)
          }
        } catch (error) {
          console.error('Error fetching matches:', error)
        }
      }
    } catch (error) {
      console.error('Error fetching group:', error)
    } finally {
      setLoading(false)
    }
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'LIVE':
        return 'bg-green-500/20 text-green-400'
      case 'FINISHED':
        return 'bg-gray-500/20 text-gray-400'
      case 'SCHEDULED':
        return 'bg-blue-500/20 text-blue-400'
      default:
        return 'bg-gray-500/20 text-gray-400'
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="text-muted-foreground">Loading group details...</div>
      </div>
    )
  }

  if (!group) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="text-muted-foreground">Group not found</div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-foreground">{group.name}</h1>
        <div className="flex gap-2 mt-2">
          <Badge className="bg-blue-500/20 text-blue-400">{group.stage}</Badge>
          <Badge variant="secondary">Tournament ID: {group.tournamentId.substring(0, 8)}</Badge>
        </div>
      </div>

      {/* Tabs */}
      <Tabs defaultValue="standings" className="w-full">
        <TabsList className="grid w-full grid-cols-3 bg-secondary">
          <TabsTrigger value="standings">Standings</TabsTrigger>
          <TabsTrigger value="matches">Matches</TabsTrigger>
          <TabsTrigger value="analysis">Analysis</TabsTrigger>
        </TabsList>

        <TabsContent value="standings" className="space-y-4 mt-6">
          {standings.length === 0 ? (
            <Card className="border-border">
              <CardContent className="py-12 text-center text-muted-foreground">
                No standings available yet
              </CardContent>
            </Card>
          ) : (
            <GroupStandings groupName={group.name} standings={standings} />
          )}
        </TabsContent>

        <TabsContent value="matches" className="space-y-4 mt-6">
          {matches.length === 0 ? (
            <Card className="border-border">
              <CardContent className="py-12 text-center text-muted-foreground">
                No matches scheduled in this group
              </CardContent>
            </Card>
          ) : (
            <div className="space-y-4">
              {matches.map((match) => (
                <Card key={match.id} className="border-border hover:bg-secondary/50 transition-colors">
                  <CardContent className="pt-6">
                    <div className="flex items-center justify-between">
                      <div className="flex-1">
                        <div className="text-sm text-muted-foreground mb-2">
                          {new Date(match.startTime).toLocaleDateString()} at{' '}
                          {new Date(match.startTime).toLocaleTimeString([], {
                            hour: '2-digit',
                            minute: '2-digit',
                          })}
                        </div>
                        <div className="grid grid-cols-3 gap-6 items-center">
                          <div>
                            <p className="font-semibold text-foreground">{match.homeTeamId}</p>
                          </div>

                          {match.status === 'FINISHED' || match.status === 'LIVE' ? (
                            <div className="text-center">
                              <p className="text-3xl font-bold text-primary">
                                {match.homeTeamScore} - {match.awayTeamScore}
                              </p>
                            </div>
                          ) : (
                            <div className="text-center">
                              <p className="text-muted-foreground text-sm">vs</p>
                            </div>
                          )}

                          <div className="text-right">
                            <p className="font-semibold text-foreground">{match.awayTeamId}</p>
                          </div>
                        </div>
                      </div>

                      <div className="flex items-center gap-4 ml-6">
                        <Badge className={getStatusColor(match.status)}>{match.status}</Badge>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}
        </TabsContent>

        <TabsContent value="analysis" className="space-y-4 mt-6">
          <div className="grid grid-cols-2 gap-4">
            <Card className="border-border">
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">Total Matches</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold text-primary">{matches.length}</div>
              </CardContent>
            </Card>

            <Card className="border-border">
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">Completed</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold text-primary">
                  {matches.filter((m) => m.status === 'FINISHED').length}
                </div>
              </CardContent>
            </Card>

            <Card className="border-border">
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">Teams</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold text-primary">{standings.length}</div>
              </CardContent>
            </Card>

            <Card className="border-border">
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">Total Goals</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold text-primary">
                  {matches.reduce((sum, m) => sum + (m.homeTeamScore || 0) + (m.awayTeamScore || 0), 0)}
                </div>
              </CardContent>
            </Card>
          </div>

          <Card className="border-border">
            <CardHeader>
              <CardTitle>Group Summary</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              {standings.length > 0 && (
                <>
                  <div>
                    <p className="text-sm text-muted-foreground mb-2">Top Scorer in Group</p>
                    <p className="text-lg font-semibold text-foreground">{standings[0].teamName}</p>
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground mb-2">Leader</p>
                    <div className="flex items-center justify-between">
                      <span className="font-medium text-foreground">{standings[0].teamName}</span>
                      <span className="text-primary font-bold">{standings[0].points} pts</span>
                    </div>
                  </div>
                </>
              )}
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  )
}
