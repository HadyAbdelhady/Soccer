import { UserRole } from './enums';

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  createdAt: Date;
  updatedAt: Date;
}

export interface TeamUser {
  id: string;
  userId: string;
  teamId: string;
  joinedAt: Date;
}

export interface AdminUser {
  id: string;
  userId: string;
  createdAt: Date;
}

export interface WatcherUser {
  id: string;
  userId: string;
  createdAt: Date;
}
