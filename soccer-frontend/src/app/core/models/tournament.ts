import { TournamentType, StageType } from './enums';

export interface Tournament {
  id: string;
  name: string;
  description: string;
  startDate: Date;
  endDate: Date;
  type: TournamentType;
  createdAt: Date;
  updatedAt: Date;
}

export interface TournamentWithStats extends Tournament {
  totalMatches: number;
  totalTeams: number;
}

export interface Group {
  id: string;
  tournamentId: string;
  name: string;
  stage: StageType;
  createdAt: Date;
  updatedAt: Date;
}

export interface GroupStanding {
  id: string;
  groupId: string;
  teamId: string;
  teamName: string;
  matchesPlayed: number;
  wins: number;
  draws: number;
  losses: number;
  goalsFor: number;
  goalsAgainst: number;
  goalDifference: number;
  points: number;
}
