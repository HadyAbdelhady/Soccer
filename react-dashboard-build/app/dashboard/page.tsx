'use client'

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { useAuth } from '@/app/auth-context'

export default function DashboardPage() {
  const { user } = useAuth()

  return (
    <div className="space-y-6">
      {/* Welcome Section */}
      <div>
        <h1 className="text-3xl font-bold text-foreground mb-2">Welcome back, {user?.username}!</h1>
        <p className="text-muted-foreground">
          Manage your tournaments, teams, players, and matches all in one place.
        </p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card className="border-border">
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Active Tournaments</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-3xl font-bold text-primary">0</div>
            <p className="text-xs text-muted-foreground mt-1">Ongoing tournaments</p>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Total Teams</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-3xl font-bold text-primary">0</div>
            <p className="text-xs text-muted-foreground mt-1">Registered teams</p>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Total Players</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-3xl font-bold text-primary">0</div>
            <p className="text-xs text-muted-foreground mt-1">All players</p>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Scheduled Matches</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-3xl font-bold text-primary">0</div>
            <p className="text-xs text-muted-foreground mt-1">Upcoming matches</p>
          </CardContent>
        </Card>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <Card className="border-border lg:col-span-2">
          <CardHeader>
            <CardTitle>Recent Matches</CardTitle>
            <CardDescription>Latest match results and upcoming fixtures</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex items-center justify-center py-8 text-muted-foreground">
              <p>No matches scheduled yet. Create a tournament to get started.</p>
            </div>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardHeader>
            <CardTitle>Quick Links</CardTitle>
            <CardDescription>Navigation shortcuts</CardDescription>
          </CardHeader>
          <CardContent className="space-y-2">
            <a
              href="/dashboard/tournaments"
              className="block px-3 py-2 rounded-lg bg-secondary hover:bg-secondary/80 text-foreground text-sm font-medium transition-colors"
            >
              🏆 Create Tournament
            </a>
            <a
              href="/dashboard/teams"
              className="block px-3 py-2 rounded-lg bg-secondary hover:bg-secondary/80 text-foreground text-sm font-medium transition-colors"
            >
              👥 Register Team
            </a>
            <a
              href="/dashboard/players"
              className="block px-3 py-2 rounded-lg bg-secondary hover:bg-secondary/80 text-foreground text-sm font-medium transition-colors"
            >
              ⚽ Add Players
            </a>
          </CardContent>
        </Card>
      </div>

      {/* Top Scorers */}
      <Card className="border-border">
        <CardHeader>
          <CardTitle>Top Scorers</CardTitle>
          <CardDescription>Top 10 scorers across all tournaments</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-center py-8 text-muted-foreground">
            <p>No data available yet</p>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
