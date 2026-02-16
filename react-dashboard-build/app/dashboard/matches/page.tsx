'use client'

import { useEffect, useState } from 'react'
import { api } from '@/lib/api'
import type { Match } from '@/lib/types'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { ScheduleMatchDialog } from './schedule-match-dialog'

export default function MatchesPage() {
  const [matches, setMatches] = useState<Match[]>([])
  const [loading, setLoading] = useState(true)
  const [scheduleDialogOpen, setScheduleDialogOpen] = useState(false)
  const [selectedMatch, setSelectedMatch] = useState<Match | null>(null)

  useEffect(() => {
    fetchMatches()
  }, [])

  const fetchMatches = async () => {
    setLoading(true)
    try {
      const response = await api.get<any>('/match/getAllMatches')
      if (response.isSuccess && response.data) {
        // Backend returns { tournaments: [{ matches: [] }] }
        const allMatches: Match[] = []
        if (response.data.tournaments) {
          response.data.tournaments.forEach((t: any) => {
            if (t.matches) {
              t.matches.forEach((m: any) => {
                allMatches.push({
                  id: m.id,
                  groupId: m.groupId,
                  homeTeamId: m.homeTeamName || m.homeTeamId || 'TBD',
                  awayTeamId: m.awayTeamName || m.awayTeamId || 'TBD',
                  status: m.status,
                  startTime: m.kickoffTime || new Date().toISOString(),
                  venue: m.venue,
                  homeTeamScore: 0, // Score missing in MatchResponse
                  awayTeamScore: 0, // Score missing in MatchResponse
                  createdAt: new Date().toISOString(),
                  updatedAt: new Date().toISOString()
                })
              })
            }
          })
        }
        setMatches(allMatches)
      }
    } catch (error) {
      console.error('Error fetching matches:', error)
    } finally {
      setLoading(false)
    }
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'SCHEDULED':
        return 'bg-blue-500/20 text-blue-400'
      case 'LIVE':
        return 'bg-green-500/20 text-green-400'
      case 'FINISHED':
        return 'bg-gray-500/20 text-gray-400'
      case 'POSTPONED':
        return 'bg-yellow-500/20 text-yellow-400'
      case 'CANCELLED':
        return 'bg-red-500/20 text-red-400'
      default:
        return 'bg-gray-500/20 text-gray-400'
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Matches</h1>
          <p className="text-muted-foreground mt-1">Schedule and manage matches</p>
        </div>
        <Button className="bg-primary hover:bg-primary/90">+ Schedule Match</Button>
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-12">
          <div className="text-muted-foreground">Loading matches...</div>
        </div>
      ) : matches.length === 0 ? (
        <Card className="border-border">
          <CardContent className="py-12 text-center">
            <div className="text-4xl mb-4">🎯</div>
            <h3 className="text-xl font-semibold text-foreground mb-2">No matches scheduled yet</h3>
            <p className="text-muted-foreground mb-4">Create a tournament to schedule matches</p>
            <Button className="bg-primary hover:bg-primary/90">Schedule Match</Button>
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
                        <p className="font-semibold text-foreground">Home Team</p>
                        <p className="text-muted-foreground text-sm mt-1">{match.homeTeamId}</p>
                      </div>

                      <div className="text-center">
                        {match.status === 'FINISHED' || match.status === 'LIVE' ? (
                          <div className="bg-secondary px-4 py-2 rounded-lg">
                            <p className="text-2xl font-bold text-foreground">
                              {match.homeTeamScore} - {match.awayTeamScore}
                            </p>
                          </div>
                        ) : (
                          <p className="text-muted-foreground text-sm">vs</p>
                        )}
                      </div>

                      <div className="text-right">
                        <p className="font-semibold text-foreground">Away Team</p>
                        <p className="text-muted-foreground text-sm mt-1">{match.awayTeamId}</p>
                      </div>
                    </div>
                  </div>

                  <div className="flex items-center gap-4 ml-6">
                    <Badge className={getStatusColor(match.status)}>{match.status}</Badge>
                    <Button variant="outline" size="sm" className="border-border text-primary hover:bg-secondary">
                      Details
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}
