import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from '../models';
import { StorageService } from './storage.service';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private storageService: StorageService,
    private apiService: ApiService
  ) {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const token = this.storageService.getToken();
    const user = this.storageService.getUser();
    if (token && user) {
      this.currentUserSubject.next(user);
      this.isAuthenticatedSubject.next(true);
    }
  }

  login(email: string, password: string): Observable<any> {
    return new Observable(observer => {
      this.apiService.post<any>('/auth/login', { email, password }).subscribe(
        response => {
          if (response.isSuccess && response.data) {
            const { token, user } = response.data;
            this.storageService.setToken(token);
            this.storageService.setUser(user);
            this.currentUserSubject.next(user);
            this.isAuthenticatedSubject.next(true);
            observer.next(response.data);
            observer.complete();
          } else {
            observer.error(response.errorMessage);
          }
        },
        error => observer.error(error)
      );
    });
  }

  register(userData: any): Observable<any> {
    return new Observable(observer => {
      this.apiService.post<any>('/auth/register', userData).subscribe(
        response => {
          if (response.isSuccess && response.data) {
            const { token, user } = response.data;
            this.storageService.setToken(token);
            this.storageService.setUser(user);
            this.currentUserSubject.next(user);
            this.isAuthenticatedSubject.next(true);
            observer.next(response.data);
            observer.complete();
          } else {
            observer.error(response.errorMessage);
          }
        },
        error => observer.error(error)
      );
    });
  }

  logout(): void {
    this.storageService.clearAll();
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  getToken(): string | null {
    return this.storageService.getToken();
  }
}
