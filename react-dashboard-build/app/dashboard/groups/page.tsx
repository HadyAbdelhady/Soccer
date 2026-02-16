'use client'

import { useEffect, useState } from 'react'
import { api } from '@/lib/api'
import type { Group } from '@/lib/types'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'

export default function GroupsPage() {
  const [groups, setGroups] = useState<Group[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchGroups()
  }, [])

  const fetchGroups = async () => {
    setLoading(true)
    try {
      const response = await api.get<Group[]>('/group')
      if (response.isSuccess && response.data) {
        setGroups(response.data)
      }
    } catch (error) {
      console.error('Error fetching groups:', error)
    } finally {
      setLoading(false)
    }
  }

  const getStageColor = (stage: string) => {
    switch (stage) {
      case 'GROUP':
        return 'bg-blue-500/20 text-blue-400'
      case 'KNOCKOUT':
        return 'bg-red-500/20 text-red-400'
      default:
        return 'bg-gray-500/20 text-gray-400'
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Groups</h1>
          <p className="text-muted-foreground mt-1">Tournament groups and standings</p>
        </div>
        <Button className="bg-primary hover:bg-primary/90">+ Create Group</Button>
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-12">
          <div className="text-muted-foreground">Loading groups...</div>
        </div>
      ) : groups.length === 0 ? (
        <Card className="border-border">
          <CardContent className="py-12 text-center">
            <div className="text-4xl mb-4">📋</div>
            <h3 className="text-xl font-semibold text-foreground mb-2">No groups created yet</h3>
            <p className="text-muted-foreground mb-4">Create a tournament to organize groups</p>
            <Button className="bg-primary hover:bg-primary/90">Create Group</Button>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {groups.map((group) => (
            <a key={group.id} href={`/dashboard/groups/${group.id}`}>
              <Card className="border-border hover:bg-secondary/50 transition-colors cursor-pointer h-full">
                <CardHeader>
                  <div className="flex items-start justify-between">
                    <div>
                      <CardTitle className="text-lg text-foreground">{group.name}</CardTitle>
                      <CardDescription className="mt-1">
                        Tournament: {group.tournamentId.substring(0, 8)}
                      </CardDescription>
                    </div>
                    <Badge className={getStageColor(group.stage)}>{group.stage}</Badge>
                  </div>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    <div>
                      <p className="text-sm text-muted-foreground mb-2">Group Standings</p>
                      <div className="bg-secondary rounded-lg p-3">
                        <p className="text-sm text-muted-foreground text-center">View full standings</p>
                      </div>
                    </div>
                    <Button variant="outline" className="w-full border-border text-primary hover:bg-secondary">
                      View Details
                    </Button>
                  </div>
                </CardContent>
              </Card>
            </a>
          ))}
        </div>
      )}
    </div>
  )
}
