'use client'

import { useEffect, useState } from 'react'
import { api } from '@/lib/api'
import type { Player, Team } from '@/lib/types'
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { PlayerForm } from '@/components/player-form'

export default function PlayersPage() {
  const [players, setPlayers] = useState<Player[]>([])
  const [teams, setTeams] = useState<Team[]>([])
  const [loading, setLoading] = useState(true)
  const [selectedTeam, setSelectedTeam] = useState<string>('all')
  const [showForm, setShowForm] = useState(false)

  useEffect(() => {
    fetchPlayers()
    fetchTeams()
  }, [])

  const fetchTeams = async () => {
    try {
      const response = await api.get<any>('/team')
      if (response.isSuccess && response.data) {
        // Backend returns { teams: [] }
        const teamsData = response.data.teams || []
        const mappedTeams: Team[] = teamsData.map((t: any) => ({
          id: t.id,
          name: t.name,
          username: t.username,
          country: '',
          founded: undefined,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }))
        setTeams(mappedTeams)
      }
    } catch (error) {
      console.error('Error fetching teams:', error)
    }
  }

  const fetchPlayers = async () => {
    setLoading(true)
    try {
      const response = await api.get<any>('/player')
      if (response.isSuccess && response.data) {
        // Backend returns array of GetPlayerResponse
        const playersData = Array.isArray(response.data) ? response.data : []
        const mappedPlayers: Player[] = playersData.map((p: any) => ({
          id: p.id,
          name: p.fullName || p.nickName || '',
          shirtNumber: p.jerseyNumber || 0,
          position: p.position,
          nationality: '',
          teamId: p.teamId,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }))
        setPlayers(mappedPlayers)
      }
    } catch (error) {
      console.error('Error fetching players:', error)
    } finally {
      setLoading(false)
    }
  }

  const handlePlayerAdded = (newPlayer: Player) => {
    setPlayers([...players, newPlayer])
    setShowForm(false)
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

  const filteredPlayers = selectedTeam && selectedTeam !== 'all' ? players.filter((p) => p.teamId === selectedTeam) : players

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Players</h1>
          <p className="text-muted-foreground mt-1">Manage all players in your teams</p>
        </div>
        <Dialog open={showForm} onOpenChange={setShowForm}>
          <DialogTrigger asChild>
            <Button className="bg-primary hover:bg-primary/90">+ Add Player</Button>
          </DialogTrigger>
          <DialogContent className="max-w-md border-border bg-card">
            <DialogHeader>
              <DialogTitle>Add Player</DialogTitle>
              <DialogDescription>Register a new player to a team</DialogDescription>
            </DialogHeader>
            {selectedTeam && selectedTeam !== 'all' && <PlayerForm teamId={selectedTeam} onSuccess={handlePlayerAdded} />}
            {(!selectedTeam || selectedTeam === 'all') && (
              <div className="space-y-4">
                <p className="text-sm text-muted-foreground">Select a team first from the filter below</p>
              </div>
            )}
          </DialogContent>
        </Dialog>
      </div>

      {/* Filter by Team */}
      <div className="flex items-center gap-2">
        <span className="text-sm text-muted-foreground">Filter by team:</span>
        <Select value={selectedTeam} onValueChange={setSelectedTeam}>
          <SelectTrigger className="w-48 bg-secondary border-border">
            <SelectValue placeholder="All teams" />
          </SelectTrigger>
          <SelectContent className="bg-card border-border">
            <SelectItem value="all">All teams</SelectItem>
            {teams.map((team) => (
              <SelectItem key={team.id} value={team.id}>
                {team.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-12">
          <div className="text-muted-foreground">Loading players...</div>
        </div>
      ) : players.length === 0 ? (
        <Card className="border-border">
          <CardContent className="py-12 text-center">
            <div className="text-4xl mb-4">⚽</div>
            <h3 className="text-xl font-semibold text-foreground mb-2">No players registered yet</h3>
            <p className="text-muted-foreground mb-4">Add players to your teams to get started</p>
            <Dialog open={showForm} onOpenChange={setShowForm}>
              <DialogTrigger asChild>
                <Button className="bg-primary hover:bg-primary/90">Add Player</Button>
              </DialogTrigger>
              <DialogContent className="max-w-md border-border bg-card">
                <DialogHeader>
                  <DialogTitle>Add Player</DialogTitle>
                  <DialogDescription>Register a new player to a team</DialogDescription>
                </DialogHeader>
                {selectedTeam && selectedTeam !== 'all' && <PlayerForm teamId={selectedTeam} onSuccess={handlePlayerAdded} />}
              </DialogContent>
            </Dialog>
          </CardContent>
        </Card>
      ) : (
        <Card className="border-border">
          <CardHeader>
            <CardTitle>All Players</CardTitle>
            <CardDescription>Total players: {filteredPlayers.length}</CardDescription>
          </CardHeader>
          <CardContent>
            {filteredPlayers.length === 0 ? (
              <div className="text-center py-8 text-muted-foreground">No players in selected team</div>
            ) : (
              <div className="overflow-x-auto">
                <Table>
                  <TableHeader>
                    <TableRow className="border-border hover:bg-transparent">
                      <TableHead className="text-muted-foreground">Name</TableHead>
                      <TableHead className="text-muted-foreground text-center">Shirt #</TableHead>
                      <TableHead className="text-muted-foreground text-center">Position</TableHead>
                      <TableHead className="text-muted-foreground">Nationality</TableHead>
                      <TableHead className="text-muted-foreground text-right">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {filteredPlayers.map((player) => (
                      <TableRow key={player.id} className="border-border hover:bg-secondary/50">
                        <TableCell className="font-medium text-foreground">{player.name}</TableCell>
                        <TableCell className="text-center text-foreground font-bold">{player.shirtNumber}</TableCell>
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
      )}
    </div>
  )
}
