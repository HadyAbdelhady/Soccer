'use client'

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import type { GroupStanding } from '@/lib/types'

interface GroupStandingsProps {
  groupName: string
  standings: GroupStanding[]
}

export function GroupStandings({ groupName, standings }: GroupStandingsProps) {
  const sortedStandings = [...standings].sort((a, b) => {
    if (b.points !== a.points) {
      return b.points - a.points
    }
    return b.goalDifference - a.goalDifference
  })

  return (
    <Card className="border-border">
      <CardHeader>
        <CardTitle className="text-xl">{groupName}</CardTitle>
        <CardDescription>League standings</CardDescription>
      </CardHeader>
      <CardContent>
        {standings.length === 0 ? (
          <div className="text-center py-8 text-muted-foreground">No standings available yet</div>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow className="border-border hover:bg-transparent">
                  <TableHead className="text-muted-foreground text-center w-8">Pos</TableHead>
                  <TableHead className="text-muted-foreground">Team</TableHead>
                  <TableHead className="text-muted-foreground text-center">P</TableHead>
                  <TableHead className="text-muted-foreground text-center">W</TableHead>
                  <TableHead className="text-muted-foreground text-center">D</TableHead>
                  <TableHead className="text-muted-foreground text-center">L</TableHead>
                  <TableHead className="text-muted-foreground text-center">GF</TableHead>
                  <TableHead className="text-muted-foreground text-center">GA</TableHead>
                  <TableHead className="text-muted-foreground text-center">GD</TableHead>
                  <TableHead className="text-muted-foreground text-center font-bold">Pts</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {sortedStandings.map((standing, index) => (
                  <TableRow
                    key={standing.teamId}
                    className={`border-border hover:bg-secondary/50 ${
                      index < 2 ? 'bg-green-500/5' : index < sortedStandings.length - 1 ? '' : 'bg-red-500/5'
                    }`}
                  >
                    <TableCell className="text-center font-bold text-foreground">{index + 1}</TableCell>
                    <TableCell className="font-medium text-foreground">{standing.teamName}</TableCell>
                    <TableCell className="text-center text-foreground">{standing.played}</TableCell>
                    <TableCell className="text-center text-foreground">{standing.wins}</TableCell>
                    <TableCell className="text-center text-foreground">{standing.draws}</TableCell>
                    <TableCell className="text-center text-foreground">{standing.losses}</TableCell>
                    <TableCell className="text-center text-foreground">{standing.goalsFor}</TableCell>
                    <TableCell className="text-center text-foreground">{standing.goalsAgainst}</TableCell>
                    <TableCell className="text-center text-foreground">{standing.goalDifference > 0 ? `+${standing.goalDifference}` : standing.goalDifference}</TableCell>
                    <TableCell className="text-center font-bold text-primary">{standing.points}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </CardContent>
    </Card>
  )
}
