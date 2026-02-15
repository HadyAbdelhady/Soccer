import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, Tournament, Group, GroupStanding, TournamentWithStats } from '../models';

@Injectable({
  providedIn: 'root'
})
export class TournamentService {
  constructor(private http: HttpClient) {}

  getTournaments(): Observable<Result<Tournament[]>> {
    return this.http.get<Result<Tournament[]>>('/tournaments');
  }

  getTournament(id: string): Observable<Result<TournamentWithStats>> {
    return this.http.get<Result<TournamentWithStats>>(`/tournaments/${id}`);
  }

  createTournament(tournament: Partial<Tournament>): Observable<Result<Tournament>> {
    return this.http.post<Result<Tournament>>('/tournaments', tournament);
  }

  updateTournament(id: string, tournament: Partial<Tournament>): Observable<Result<Tournament>> {
    return this.http.put<Result<Tournament>>(`/tournaments/${id}`, tournament);
  }

  deleteTournament(id: string): Observable<Result<void>> {
    return this.http.delete<Result<void>>(`/tournaments/${id}`);
  }

  getGroups(tournamentId: string): Observable<Result<Group[]>> {
    return this.http.get<Result<Group[]>>(`/tournaments/${tournamentId}/groups`);
  }

  getGroupStandings(groupId: string): Observable<Result<GroupStanding[]>> {
    return this.http.get<Result<GroupStanding[]>>(`/groups/${groupId}/standings`);
  }

  getTopScorers(tournamentId: string): Observable<Result<any[]>> {
    return this.http.get<Result<any[]>>(`/tournaments/${tournamentId}/top-scorers`);
  }
}
