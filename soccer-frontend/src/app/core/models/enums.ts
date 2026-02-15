export enum UserRole {
  Admin = 'Admin',
  TeamManager = 'TeamManager',
  Viewer = 'Viewer'
}

export enum PlayerPosition {
  Goalkeeper = 'Goalkeeper',
  Defender = 'Defender',
  Midfielder = 'Midfielder',
  Forward = 'Forward'
}

export enum MatchStatus {
  Scheduled = 'Scheduled',
  InProgress = 'InProgress',
  Finished = 'Finished',
  Cancelled = 'Cancelled'
}

export enum CardType {
  Yellow = 'Yellow',
  Red = 'Red'
}

export enum GoalType {
  Regular = 'Regular',
  OwnGoal = 'OwnGoal',
  PenaltyGoal = 'PenaltyGoal'
}

export enum TournamentType {
  LeagueFormat = 'LeagueFormat',
  KnockoutFormat = 'KnockoutFormat',
  MixedFormat = 'MixedFormat'
}

export enum LegsType {
  SingleLeg = 'SingleLeg',
  DoubleLeg = 'DoubleLeg'
}

export enum StageType {
  Group = 'Group',
  Quarterfinal = 'Quarterfinal',
  Semifinal = 'Semifinal',
  Final = 'Final'
}
