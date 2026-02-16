'use client'

import { useEffect, useState } from 'react'
import { api } from '@/lib/api'
import type { Match, MatchGoal, MatchCard, MatchLineup } from '@/lib/types'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { MatchScoresheet } from '@/components/match-scoresheet'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'

interface MatchDetailPageProps {
  params: {
    id: string
  }
}

export default function MatchDetailPage({ params }: MatchDetailPageProps) {
  const [match, setMatch] = useState<Match | null>(null)
  const [goals, setGoals] = useState<MatchGoal[]>([])
  const [cards, setCards] = useState<MatchCard[]>([])
  const [lineups, setLineups] = useState<MatchLineup[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchMatchDetails()
  }, [params.id])

  const fetchMatchDetails = async () => {
    setLoading(true)
    try {
      const matchResponse = await api.get<Match>(`/match/${params.id}`)
      if (matchResponse.isSuccess && matchResponse.data) {
        setMatch(matchResponse.data)

        // Fetch goals
        try {
          const goalsResponse = await api.get<MatchGoal[]>(`/match/${params.id}/goals`)
          if (goalsResponse.isSuccess && goalsResponse.data) {
            setGoals(goalsResponse.data)
          }
        } catch (error) {
          console.error('Error fetching goals:', error)
        }

        // Fetch cards
        try {
          const cardsResponse = await api.get<MatchCard[]>(`/match/${params.id}/cards`)
          if (cardsResponse.isSuccess && cardsResponse.data) {
            setCards(cardsResponse.data)
          }
        } catch (error) {
          console.error('Error fetching cards:', error)
        }

        // Fetch lineups
        try {
          const lineupsResponse = await api.get<MatchLineup[]>(`/match/${params.id}/lineup`)
          if (lineupsResponse.isSuccess && lineupsResponse.data) {
            setLineups(lineupsResponse.data)
          }
        } catch (error) {
          console.error('Error fetching lineups:', error)
        }
      }
    } catch (error) {
      console.error('Error fetching match:', error)
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="text-muted-foreground">Loading match details...</div>
      </div>
    )
  }

  if (!match) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="text-muted-foreground">Match not found</div>
      </div>
    )
  }

  const homeTeamLineups = lineups.filter((l) => l.teamId === match.homeTeamId)
  const awayTeamLineups = lineups.filter((l) => l.teamId === match.awayTeamId)

  return (
    <div className="space-y-6">
      {/* Main Scoresheet */}
      <MatchScoresheet match={match} goals={goals} cards={cards} />

      {/* Detailed Stats and Lineups */}
      <Tabs defaultValue="lineups" className="w-full">
        <TabsList className="grid w-full grid-cols-3 bg-secondary">
          <TabsTrigger value="lineups">Lineups</TabsTrigger>
          <TabsTrigger value="stats">Match Statistics</TabsTrigger>
          <TabsTrigger value="timeline">Timeline</TabsTrigger>
        </TabsList>

        <TabsContent value="lineups" className="space-y-4 mt-6">
          <div className="grid grid-cols-2 gap-4">
            {/* Home Team Lineup */}
            <Card className="border-border">
              <CardHeader>
                <CardTitle className="text-lg">{match.homeTeamId}</CardTitle>
                <CardDescription>Starting XI</CardDescription>
              </CardHeader>
              <CardContent>
                {homeTeamLineups.length === 0 ? (
                  <div className="text-center py-8 text-muted-foreground">No lineup set</div>
                ) : (
                  <div className="space-y-2">
                    {homeTeamLineups.filter((l) => !l.isBench).map((lineup) => (
                      <div
                        key={lineup.id}
                        className="flex items-center justify-between p-2 bg-secondary rounded"
                      >
                        <span className="text-foreground">{lineup.position}</span>
                        <span className="text-muted-foreground text-sm">Player ID: {lineup.playerId}</span>
                      </div>
                    ))}
                    {homeTeamLineups.filter((l) => l.isBench).length > 0 && (
                      <div className="mt-4">
                        <p className="text-xs text-muted-foreground mb-2">Substitutes</p>
                        {homeTeamLineups.filter((l) => l.isBench).map((lineup) => (
                          <div
                            key={lineup.id}
                            className="flex items-center justify-between p-2 bg-secondary/50 rounded text-sm"
                          >
                            <span className="text-muted-foreground">{lineup.position}</span>
                            <span className="text-muted-foreground text-xs">Player ID: {lineup.playerId}</span>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Away Team Lineup */}
            <Card className="border-border">
              <CardHeader>
                <CardTitle className="text-lg">{match.awayTeamId}</CardTitle>
                <CardDescription>Starting XI</CardDescription>
              </CardHeader>
              <CardContent>
                {awayTeamLineups.length === 0 ? (
                  <div className="text-center py-8 text-muted-foreground">No lineup set</div>
                ) : (
                  <div className="space-y-2">
                    {awayTeamLineups.filter((l) => !l.isBench).map((lineup) => (
                      <div
                        key={lineup.id}
                        className="flex items-center justify-between p-2 bg-secondary rounded"
                      >
                        <span className="text-foreground">{lineup.position}</span>
                        <span className="text-muted-foreground text-sm">Player ID: {lineup.playerId}</span>
                      </div>
                    ))}
                    {awayTeamLineups.filter((l) => l.isBench).length > 0 && (
                      <div className="mt-4">
                        <p className="text-xs text-muted-foreground mb-2">Substitutes</p>
                        {awayTeamLineups.filter((l) => l.isBench).map((lineup) => (
                          <div
                            key={lineup.id}
                            className="flex items-center justify-between p-2 bg-secondary/50 rounded text-sm"
                          >
                            <span className="text-muted-foreground">{lineup.position}</span>
                            <span className="text-muted-foreground text-xs">Player ID: {lineup.playerId}</span>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                )}
              </CardContent>
            </Card>
          </div>
        </TabsContent>

        <TabsContent value="stats" className="space-y-4 mt-6">
          <Card className="border-border">
            <CardHeader>
              <CardTitle>Match Statistics</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-3 gap-4">
                <div className="text-center">
                  <p className="text-2xl font-bold text-foreground">{match.homeTeamScore || 0}</p>
                  <p className="text-sm text-muted-foreground">{match.homeTeamId}</p>
                </div>
                <div className="text-center">
                  <p className="text-sm text-muted-foreground">Goals</p>
                  <p className="text-xs text-muted-foreground">({goals.length} total)</p>
                </div>
                <div className="text-center">
                  <p className="text-2xl font-bold text-foreground">{match.awayTeamScore || 0}</p>
                  <p className="text-sm text-muted-foreground">{match.awayTeamId}</p>
                </div>
              </div>

              <div className="mt-6 pt-6 border-t border-border">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm text-muted-foreground mb-2">Yellow Cards: {cards.filter((c) => c.type === 'YELLOW').length}</p>
                    <p className="text-sm text-muted-foreground mb-2">Red Cards: {cards.filter((c) => c.type === 'RED').length}</p>
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground mb-2">Total Cards: {cards.length}</p>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="timeline" className="space-y-4 mt-6">
          <Card className="border-border">
            <CardHeader>
              <CardTitle>Match Events</CardTitle>
            </CardHeader>
            <CardContent>
              {goals.length === 0 && cards.length === 0 ? (
                <div className="text-center py-8 text-muted-foreground">No events recorded yet</div>
              ) : (
                <div className="space-y-3">
                  {[...goals, ...cards]
                    .sort((a: any, b: any) => a.minute - b.minute)
                    .map((event: any, idx) => (
                      <div key={idx} className="flex items-center gap-4 p-3 bg-secondary rounded">
                        <div className="w-12 h-12 rounded-full bg-primary/20 flex items-center justify-center text-sm font-bold text-primary">
                          {event.minute}'
                        </div>
                        <div className="flex-1">
                          <p className="font-medium text-foreground">
                            {'type' in event && event.type === 'YELLOW'
                              ? 'Yellow Card'
                              : 'type' in event && event.type === 'RED'
                                ? 'Red Card'
                                : 'Goal'}
                          </p>
                          <p className="text-sm text-muted-foreground">Player ID: {event.playerId}</p>
                        </div>
                      </div>
                    ))}
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  )
}
