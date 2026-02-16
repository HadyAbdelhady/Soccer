'use client'

import { useAuth } from '@/app/auth-context'
import { useRouter } from 'next/navigation'
import { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { Sidebar } from '@/components/ui/sidebar'
import Link from 'next/link'

const navItems = [
  { label: 'Dashboard', href: '/dashboard', icon: '📊' },
  { label: 'Tournaments', href: '/dashboard/tournaments', icon: '🏆' },
  { label: 'Teams', href: '/dashboard/teams', icon: '👥' },
  { label: 'Players', href: '/dashboard/players', icon: '⚽' },
  { label: 'Matches', href: '/dashboard/matches', icon: '🎯' },
  { label: 'Groups', href: '/dashboard/groups', icon: '📋' },
]

export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, user, logout } = useAuth()
  const router = useRouter()
  const [mounted, setMounted] = useState(false)

  useEffect(() => {
    setMounted(true)
    if (!isAuthenticated && mounted) {
      router.push('/login')
    }
  }, [isAuthenticated, mounted, router])

  if (!mounted || !isAuthenticated) {
    return null
  }

  return (
    <div className="flex h-screen bg-background text-foreground">
      {/* Sidebar */}
      <div className="w-64 border-r border-border bg-card p-6 flex flex-col">
        <div className="mb-8">
          <div className="flex items-center gap-3 mb-4">
            <div className="w-10 h-10 bg-primary rounded-lg flex items-center justify-center text-primary-foreground font-bold">
              ⚽
            </div>
            <h1 className="text-xl font-bold text-primary">SoccerTM</h1>
          </div>
          <p className="text-xs text-muted-foreground">Tournament Management</p>
        </div>

        <nav className="space-y-1 flex-1">
          {navItems.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              className="flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-secondary transition-colors"
            >
              <span>{item.icon}</span>
              {item.label}
            </Link>
          ))}
        </nav>

        <div className="border-t border-border pt-4 space-y-3">
          <div className="px-2 py-2">
            <p className="text-xs text-muted-foreground mb-1">User Role</p>
            <p className="text-sm font-medium text-foreground capitalize">{user?.role}</p>
          </div>
          <Button
            onClick={logout}
            className="w-full bg-destructive hover:bg-destructive/90 text-destructive-foreground"
          >
            Logout
          </Button>
        </div>
      </div>

      {/* Main Content */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Top Bar */}
        <div className="border-b border-border bg-card p-6 flex items-center justify-between">
          <h2 className="text-2xl font-bold text-foreground">Dashboard</h2>
          <div className="flex items-center gap-4">
            <div className="text-right">
              <p className="text-sm font-medium text-foreground">{user?.username}</p>
              <p className="text-xs text-muted-foreground">{user?.email}</p>
            </div>
            <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center text-primary-foreground font-bold">
              {user?.username?.charAt(0).toUpperCase()}
            </div>
          </div>
        </div>

        {/* Content Area */}
        <div className="flex-1 overflow-auto p-6">{children}</div>
      </div>
    </div>
  )
}
