import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Result, PaginatedResult } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string, params?: HttpParams | any): Observable<Result<T>> {
    return this.http.get<Result<T>>(`${this.baseUrl}${endpoint}`, { params });
  }

  getList<T>(endpoint: string, params?: HttpParams | any): Observable<PaginatedResult<T>> {
    return this.http.get<PaginatedResult<T>>(`${this.baseUrl}${endpoint}`, { params });
  }

  post<T>(endpoint: string, body: any): Observable<Result<T>> {
    return this.http.post<Result<T>>(`${this.baseUrl}${endpoint}`, body);
  }

  put<T>(endpoint: string, body: any): Observable<Result<T>> {
    return this.http.put<Result<T>>(`${this.baseUrl}${endpoint}`, body);
  }

  delete<T>(endpoint: string): Observable<Result<T>> {
    return this.http.delete<Result<T>>(`${this.baseUrl}${endpoint}`);
  }

  patch<T>(endpoint: string, body: any): Observable<Result<T>> {
    return this.http.patch<Result<T>>(`${this.baseUrl}${endpoint}`, body);
  }
}
