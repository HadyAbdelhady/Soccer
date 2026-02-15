import { MatchStatus } from './enums';

export interface Match {
  id: string;
  groupId: string;
  homeTeamId: string;
  awayTeamId: string;
  homeTeamName?: string;
  awayTeamName?: string;
  homeTeamGoals: number;
  awayTeamGoals: number;
  status: MatchStatus;
  matchDate: Date;
  createdAt: Date;
  updatedAt: Date;
}

export interface MatchDetail extends Match {
  goals: MatchGoal[];
  cards: MatchCard[];
  lineups: MatchLineup[];
}

export interface MatchLineup {
  id: string;
  matchId: string;
  teamId: string;
  playerId: string;
  playerName?: string;
  jerseyNumber?: number;
  position: string;
  isStarting: boolean;
  createdAt: Date;
}

export interface MatchGoal {
  id: string;
  matchId: string;
  teamId: string;
  playerId: string;
  playerName?: string;
  minute: number;
  type: string;
  createdAt: Date;
}

export interface MatchCard {
  id: string;
  matchId: string;
  teamId: string;
  playerId: string;
  playerName?: string;
  cardType: string;
  minute: number;
  createdAt: Date;
}
