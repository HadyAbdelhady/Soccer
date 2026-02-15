import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, Team, Player, TeamWithPlayers } from '../models';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  constructor(private http: HttpClient) {}

  getTeams(): Observable<Result<Team[]>> {
    return this.http.get<Result<Team[]>>('/teams');
  }

  getTeam(id: string): Observable<Result<TeamWithPlayers>> {
    return this.http.get<Result<TeamWithPlayers>>(`/teams/${id}`);
  }

  createTeam(team: Partial<Team>): Observable<Result<Team>> {
    return this.http.post<Result<Team>>('/teams', team);
  }

  updateTeam(id: string, team: Partial<Team>): Observable<Result<Team>> {
    return this.http.put<Result<Team>>(`/teams/${id}`, team);
  }

  deleteTeam(id: string): Observable<Result<void>> {
    return this.http.delete<Result<void>>(`/teams/${id}`);
  }

  getTeamPlayers(teamId: string): Observable<Result<Player[]>> {
    return this.http.get<Result<Player[]>>(`/teams/${teamId}/players`);
  }
}
