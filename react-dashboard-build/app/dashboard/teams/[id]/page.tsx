'use client'

import { useEffect, useState } from 'react'
import { api } from '@/lib/api'
import type { Team, Player } from '@/lib/types'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog'
import { PlayerForm } from '@/components/player-form'

interface TeamDetailPageProps {
  params: {
    id: string
  }
}

export default function TeamDetailPage({ params }: TeamDetailPageProps) {
  const [team, setTeam] = useState<Team | null>(null)
  const [players, setPlayers] = useState<Player[]>([])
  const [loading, setLoading] = useState(true)
  const [showPlayerForm, setShowPlayerForm] = useState(false)

  useEffect(() => {
    fetchTeam()
    fetchPlayers()
  }, [params.id])

  const fetchTeam = async () => {
    try {
      const response = await api.get<Team>(`/team/${params.id}`)
      if (response.isSuccess && response.data) {
        setTeam(response.data)
      }
    } catch (error) {
      console.error('Error fetching team:', error)
    }
  }

  const fetchPlayers = async () => {
    setLoading(true)
    try {
      const response = await api.get<Player[]>(`/player?teamId=${params.id}`)
      if (response.isSuccess && response.data) {
        setPlayers(response.data)
      }
    } catch (error) {
      console.error('Error fetching players:', error)
    } finally {
      setLoading(false)
    }
  }

  const handlePlayerAdded = (newPlayer: Player) => {
    setPlayers([...players, newPlayer])
    setShowPlayerForm(false)
  }

  const getPositionColor = (position: string) => {
    switch (position) {
      case 'GOALKEEPER':
        return 'bg-yellow-500/20 text-yellow-400'
      case 'DEFENDER':
        return 'bg-blue-500/20 text-blue-400'
      case 'MIDFIELDER':
        return 'bg-green-500/20 text-green-400'
      case 'FORWARD':
        return 'bg-red-500/20 text-red-400'
      default:
        return 'bg-gray-500/20 text-gray-400'
    }
  }

  const getPositionLabel = (position: string) => {
    switch (position) {
      case 'GOALKEEPER':
        return 'GK'
      case 'DEFENDER':
        return 'DEF'
      case 'MIDFIELDER':
        return 'MID'
      case 'FORWARD':
        return 'FWD'
      default:
        return position
    }
  }

  if (!team) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="text-muted-foreground">Loading team...</div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div className="flex items-center gap-4">
          <div className="w-16 h-16 bg-primary rounded-lg flex items-center justify-center text-primary-foreground font-bold text-2xl">
            {team.name.charAt(0)}
          </div>
          <div>
            <h1 className="text-3xl font-bold text-foreground">{team.name}</h1>
            {team.country && <p className="text-muted-foreground mt-1">{team.country}</p>}
            {team.founded && <p className="text-sm text-muted-foreground">Founded {team.founded}</p>}
          </div>
        </div>
        <Button variant="outline" className="border-border text-primary hover:bg-secondary">
          Edit Team
        </Button>
      </div>

      {/* Team Stats */}
      <div className="grid grid-cols-3 gap-4">
        <Card className="border-border">
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Total Players</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-3xl font-bold text-primary">{players.length}</div>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Goalkeepers</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-3xl font-bold text-yellow-400">
              {players.filter((p) => p.position === 'GOALKEEPER').length}
            </div>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Outfield</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-3xl font-bold text-blue-400">
              {players.filter((p) => p.position !== 'GOALKEEPER').length}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Players Section */}
      <Card className="border-border">
        <CardHeader className="flex items-center justify-between">
          <div>
            <CardTitle>Squad</CardTitle>
            <CardDescription>Players in this team</CardDescription>
          </div>
          <Dialog open={showPlayerForm} onOpenChange={setShowPlayerForm}>
            <DialogTrigger asChild>
              <Button className="bg-primary hover:bg-primary/90">+ Add Player</Button>
            </DialogTrigger>
            <DialogContent className="max-w-md border-border bg-card">
              <DialogHeader>
                <DialogTitle>Add Player</DialogTitle>
                <DialogDescription>Register a new player to the team</DialogDescription>
              </DialogHeader>
              <PlayerForm teamId={params.id} onSuccess={handlePlayerAdded} />
            </DialogContent>
          </Dialog>
        </CardHeader>

        <CardContent>
          {loading ? (
            <div className="text-center py-8 text-muted-foreground">Loading players...</div>
          ) : players.length === 0 ? (
            <div className="text-center py-8">
              <p className="text-muted-foreground mb-4">No players in this team yet</p>
              <Dialog open={showPlayerForm} onOpenChange={setShowPlayerForm}>
                <DialogTrigger asChild>
                  <Button className="bg-primary hover:bg-primary/90">Add First Player</Button>
                </DialogTrigger>
                <DialogContent className="max-w-md border-border bg-card">
                  <DialogHeader>
                    <DialogTitle>Add Player</DialogTitle>
                    <DialogDescription>Register a new player to the team</DialogDescription>
                  </DialogHeader>
                  <PlayerForm teamId={params.id} onSuccess={handlePlayerAdded} />
                </DialogContent>
              </Dialog>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow className="border-border hover:bg-transparent">
                    <TableHead className="text-muted-foreground text-center w-12">No.</TableHead>
                    <TableHead className="text-muted-foreground">Name</TableHead>
                    <TableHead className="text-muted-foreground text-center">Position</TableHead>
                    <TableHead className="text-muted-foreground">Nationality</TableHead>
                    <TableHead className="text-muted-foreground text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {players.map((player) => (
                    <TableRow key={player.id} className="border-border hover:bg-secondary/50">
                      <TableCell className="text-center font-bold text-foreground">{player.shirtNumber}</TableCell>
                      <TableCell className="font-medium text-foreground">{player.name}</TableCell>
                      <TableCell className="text-center">
                        <span className={`px-2 py-1 rounded text-xs font-medium ${getPositionColor(player.position)}`}>
                          {getPositionLabel(player.position)}
                        </span>
                      </TableCell>
                      <TableCell className="text-muted-foreground">{player.nationality || '-'}</TableCell>
                      <TableCell className="text-right">
                        <Button variant="outline" size="sm" className="border-border text-primary hover:bg-secondary">
                          Edit
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
