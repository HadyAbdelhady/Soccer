export interface Team {
  id: string;
  name: string;
  description: string;
  logoUrl?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface TeamWithPlayers extends Team {
  players: Player[];
}

export interface Player {
  id: string;
  teamId: string;
  firstName: string;
  lastName: string;
  jerseyNumber: number;
  position: string;
  dateOfBirth?: Date;
  nationality?: string;
  createdAt: Date;
  updatedAt: Date;
}
