'use client'

import { useState } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import type { Match, MatchGoal, MatchCard } from '@/lib/types'

interface MatchScoreSheetProps {
  match: Match
  goals?: MatchGoal[]
  cards?: MatchCard[]
}

export function MatchScoresheet({ match, goals = [], cards = [] }: MatchScoreSheetProps) {
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

  const getCardColor = (type: string) => {
    switch (type) {
      case 'YELLOW':
        return 'bg-yellow-500/20 text-yellow-400'
      case 'SECONDYELLOW':
        return 'bg-orange-500/20 text-orange-400'
      case 'RED':
        return 'bg-red-500/20 text-red-400'
      default:
        return 'bg-gray-500/20 text-gray-400'
    }
  }

  return (
    <Card className="border-border">
      <CardHeader>
        <div className="flex items-start justify-between mb-4">
          <div>
            <CardTitle className="text-2xl">Match Details</CardTitle>
            <CardDescription>
              {new Date(match.startTime).toLocaleDateString()} at{' '}
              {new Date(match.startTime).toLocaleTimeString([], {
                hour: '2-digit',
                minute: '2-digit',
              })}
            </CardDescription>
          </div>
          <Badge className={getStatusColor(match.status)}>{match.status}</Badge>
        </div>
      </CardHeader>

      <CardContent className="space-y-6">
        {/* Score Board */}
        <div className="bg-secondary rounded-lg p-6">
          <div className="grid grid-cols-3 gap-6 items-center">
            <div className="text-center">
              <p className="text-sm text-muted-foreground mb-2">Home Team</p>
              <p className="text-2xl font-bold text-foreground">{match.homeTeamId}</p>
            </div>

            {match.status === 'FINISHED' || match.status === 'LIVE' ? (
              <div className="text-center">
                <p className="text-5xl font-bold text-primary">
                  {match.homeTeamScore} - {match.awayTeamScore}
                </p>
                <p className="text-xs text-muted-foreground mt-2">Final Score</p>
              </div>
            ) : (
              <div className="text-center">
                <p className="text-2xl text-muted-foreground">vs</p>
              </div>
            )}

            <div className="text-center">
              <p className="text-sm text-muted-foreground mb-2">Away Team</p>
              <p className="text-2xl font-bold text-foreground">{match.awayTeamId}</p>
            </div>
          </div>
        </div>

        {/* Match Events */}
        {match.status === 'FINISHED' || match.status === 'LIVE' ? (
          <Tabs defaultValue="goals" className="w-full">
            <TabsList className="grid w-full grid-cols-2 bg-secondary">
              <TabsTrigger value="goals">Goals ({goals.length})</TabsTrigger>
              <TabsTrigger value="cards">Cards ({cards.length})</TabsTrigger>
            </TabsList>

            <TabsContent value="goals" className="space-y-3 mt-4">
              {goals.length === 0 ? (
                <div className="text-center py-6 text-muted-foreground">No goals recorded yet</div>
              ) : (
                <div className="space-y-2">
                  {goals.map((goal, idx) => (
                    <div key={idx} className="flex items-center justify-between p-3 bg-secondary rounded-lg">
                      <div>
                        <p className="font-medium text-foreground">Goal</p>
                        <p className="text-sm text-muted-foreground">Minute {goal.minute}'</p>
                      </div>
                      <Badge className="bg-blue-500/20 text-blue-400">{goal.type}</Badge>
                    </div>
                  ))}
                </div>
              )}
            </TabsContent>

            <TabsContent value="cards" className="space-y-3 mt-4">
              {cards.length === 0 ? (
                <div className="text-center py-6 text-muted-foreground">No cards issued</div>
              ) : (
                <div className="space-y-2">
                  {cards.map((card, idx) => (
                    <div
                      key={idx}
                      className="flex items-center justify-between p-3 bg-secondary rounded-lg"
                    >
                      <div>
                        <p className="font-medium text-foreground">Card</p>
                        <p className="text-sm text-muted-foreground">Minute {card.minute}'</p>
                      </div>
                      <Badge className={getCardColor(card.type)}>{card.type}</Badge>
                    </div>
                  ))}
                </div>
              )}
            </TabsContent>
          </Tabs>
        ) : (
          <div className="text-center py-6 text-muted-foreground">
            Match has not started yet. Events will appear once the match begins.
          </div>
        )}

        {/* Action Buttons */}
        {match.status === 'SCHEDULED' && (
          <div className="flex gap-3">
            <Button className="flex-1 bg-primary hover:bg-primary/90">Start Match</Button>
            <Button variant="outline" className="flex-1 border-border text-primary hover:bg-secondary">
              Cancel Match
            </Button>
          </div>
        )}

        {match.status === 'LIVE' && (
          <Button className="w-full bg-green-500/20 text-green-400 hover:bg-green-500/30">
            End Match
          </Button>
        )}
      </CardContent>
    </Card>
  )
}
