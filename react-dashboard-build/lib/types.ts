// Enums
export type UserRole = 'Team' | 'Admin' | 'Viewer'
export type PlayerPosition = 'GOALKEEPER' | 'DEFENDER' | 'MIDFIELDER' | 'FORWARD'
export type MatchStatus = 'SCHEDULED' | 'LIVE' | 'FINISHED' | 'POSTPONED' | 'CANCELLED' | 'ABANDONED'
export type CardType = 'YELLOW' | 'SECONDYELLOW' | 'RED'
export type GoalType = 'REGULAR' | 'PENALTY' | 'OWNGOAL' | 'FOUL'
export type TournamentType = 'SINGLE_GROUP' | 'MULTI_GROUP_KNOCKOUT'
export type LegsType = 'SINGLE' | 'DOUBLE'
export type StageType = 'GROUP' | 'KNOCKOUT'

// User Models
export interface User {
  id: string
  username: string
  email: string
  role: UserRole
  teamId?: string
  createdAt: string
  updatedAt: string
}

// Team Models
export interface Team {
  id: string
  name: string
  country?: string
  logo?: string
  founded?: number
  createdAt: string
  updatedAt: string
}

// Player Models
export interface Player {
  id: string
  teamId: string
  name: string
  shirtNumber: number
  position: PlayerPosition
  dateOfBirth?: string
  nationality?: string
  height?: number
  weight?: number
  createdAt: string
  updatedAt: string
}

// Tournament Models
export interface Tournament {
  id: string
  name: string
  type: TournamentType
  startDate: string
  endDate: string
  status: 'UPCOMING' | 'ONGOING' | 'COMPLETED'
  createdAt: string
  updatedAt: string
  teamCount?: number
}

// Group Models
export interface Group {
  id: string
  name: string
  standings?: GroupStanding[]
}

// Match Models
export interface Match {
  id: string
  groupId: string
  homeTeamId: string
  awayTeamId: string
  status: MatchStatus
  startTime: string
  venue?: string
  homeTeamScore?: number
  awayTeamScore?: number
  createdAt: string
  updatedAt: string
}

// Match Event Models
export interface MatchGoal {
  id: string
  matchId: string
  playerId: string
  teamId: string
  minute: number
  type: GoalType
  createdAt: string
  updatedAt: string
}

export interface MatchCard {
  id: string
  matchId: string
  playerId: string
  teamId: string
  minute: number
  type: CardType
  createdAt: string
  updatedAt: string
}

export interface MatchLineup {
  id: string
  matchId: string
  teamId: string
  playerId: string
  position: PlayerPosition
  isBench: boolean
  createdAt: string
  updatedAt: string
}

// Standing Models
export interface GroupStanding {
  teamId: string
  teamName: string
  played: number
  wins: number
  draws: number
  losses: number
  goalsFor: number
  goalsAgainst: number
  goalDifference: number
  points: number
}

// Statistics Models
export interface TopScorer {
  playerId: string
  playerName: string
  teamId: string
  teamName: string
  goals: number
}

export interface TournamentStats {
  totalMatches: number
  totalGoals: number
  averageGoalsPerMatch: number
  highestScoringTeam: {
    teamId: string
    teamName: string
    goals: number
  }
}
