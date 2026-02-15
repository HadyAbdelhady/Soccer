import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User, AuthResponse } from '../models';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  constructor() {}

  setToken(token: string): void {
    localStorage.setItem('auth_token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  removeToken(): void {
    localStorage.removeItem('auth_token');
  }

  setRefreshToken(token: string): void {
    localStorage.setItem('refresh_token', token);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  removeRefreshToken(): void {
    localStorage.removeItem('refresh_token');
  }

  setUser(user: User): void {
    localStorage.setItem('current_user', JSON.stringify(user));
  }

  getUser(): User | null {
    const user = localStorage.getItem('current_user');
    return user ? JSON.parse(user) : null;
  }

  removeUser(): void {
    localStorage.removeItem('current_user');
  }

  clearAll(): void {
    localStorage.clear();
  }
}
