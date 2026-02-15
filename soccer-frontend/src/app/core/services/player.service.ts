import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, Player } from '../models';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  constructor(private http: HttpClient) {}

  getPlayers(): Observable<Result<Player[]>> {
    return this.http.get<Result<Player[]>>('/players');
  }

  getPlayer(id: string): Observable<Result<Player>> {
    return this.http.get<Result<Player>>(`/players/${id}`);
  }

  createPlayer(player: Partial<Player>): Observable<Result<Player>> {
    return this.http.post<Result<Player>>('/players', player);
  }

  updatePlayer(id: string, player: Partial<Player>): Observable<Result<Player>> {
    return this.http.put<Result<Player>>(`/players/${id}`, player);
  }

  deletePlayer(id: string): Observable<Result<void>> {
    return this.http.delete<Result<void>>(`/players/${id}`);
  }
}
