'use client'

import { useEffect, useState } from 'react'
import { api } from '@/lib/api'
import type { Team } from '@/lib/types'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog'
import { TeamForm } from '@/components/team-form'

export default function TeamsPage() {
  const [teams, setTeams] = useState<Team[]>([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)

  useEffect(() => {
    fetchTeams()
  }, [])

  const fetchTeams = async () => {
    setLoading(true)
    try {
      const response = await api.get<any>('/team')
      if (response.isSuccess && response.data) {
        // Backend returns { teams: [] }
        const teamsData = response.data.teams || []
        const mappedTeams: Team[] = teamsData.map((t: any) => ({
          id: t.id,
          name: t.name,
          username: t.username,
          // Add placeholder values for fields not in backend response
          country: '',
          founded: undefined,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }))
        setTeams(mappedTeams)
      }
    } catch (error) {
      console.error('Error fetching teams:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleTeamCreated = (newTeam: Team) => {
    setTeams([...teams, newTeam])
    setShowForm(false)
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Teams</h1>
          <p className="text-muted-foreground mt-1">Manage teams and their information</p>
        </div>
        <Dialog open={showForm} onOpenChange={setShowForm}>
          <DialogTrigger asChild>
            <Button className="bg-primary hover:bg-primary/90">+ Register Team</Button>
          </DialogTrigger>
          <DialogContent className="max-w-md border-border bg-card">
            <DialogHeader>
              <DialogTitle>Register Team</DialogTitle>
              <DialogDescription>Add a new team to the system</DialogDescription>
            </DialogHeader>
            <TeamForm onSuccess={handleTeamCreated} />
          </DialogContent>
        </Dialog>
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-12">
          <div className="text-muted-foreground">Loading teams...</div>
        </div>
      ) : teams.length === 0 ? (
        <Card className="border-border">
          <CardContent className="py-12 text-center">
            <div className="text-4xl mb-4">👥</div>
            <h3 className="text-xl font-semibold text-foreground mb-2">No teams registered yet</h3>
            <p className="text-muted-foreground mb-4">Create your first team to get started</p>
            <Dialog open={showForm} onOpenChange={setShowForm}>
              <DialogTrigger asChild>
                <Button className="bg-primary hover:bg-primary/90">Register Team</Button>
              </DialogTrigger>
              <DialogContent className="max-w-md border-border bg-card">
                <DialogHeader>
                  <DialogTitle>Register Team</DialogTitle>
                  <DialogDescription>Add a new team to the system</DialogDescription>
                </DialogHeader>
                <TeamForm onSuccess={handleTeamCreated} />
              </DialogContent>
            </Dialog>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {teams.map((team) => (
            <Card key={team.id} className="border-border hover:bg-secondary/50 transition-colors cursor-pointer">
              <CardHeader>
                <div className="flex items-center gap-4">
                  <div className="w-12 h-12 bg-primary rounded-lg flex items-center justify-center text-primary-foreground font-bold text-lg">
                    {team.name.charAt(0)}
                  </div>
                  <div className="flex-1">
                    <CardTitle className="text-lg text-foreground">{team.name}</CardTitle>
                    {team.country && <CardDescription>{team.country}</CardDescription>}
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  {team.founded && (
                    <div>
                      <p className="text-xs text-muted-foreground">Founded</p>
                      <p className="text-sm text-foreground font-medium">{team.founded}</p>
                    </div>
                  )}
                  <div className="pt-2 border-t border-border">
                    <Button variant="outline" className="w-full border-border text-primary hover:bg-secondary">
                      View Team
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
