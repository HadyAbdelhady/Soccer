import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, Match, MatchDetail, MatchLineup, MatchGoal, MatchCard } from '../models';

@Injectable({
  providedIn: 'root'
})
export class MatchService {
  constructor(private http: HttpClient) {}

  getMatches(): Observable<Result<Match[]>> {
    return this.http.get<Result<Match[]>>('/matches');
  }

  getMatch(id: string): Observable<Result<MatchDetail>> {
    return this.http.get<Result<MatchDetail>>(`/matches/${id}`);
  }

  createMatch(match: Partial<Match>): Observable<Result<Match>> {
    return this.http.post<Result<Match>>('/matches', match);
  }

  updateMatch(id: string, match: Partial<Match>): Observable<Result<Match>> {
    return this.http.put<Result<Match>>(`/matches/${id}`, match);
  }

  deleteMatch(id: string): Observable<Result<void>> {
    return this.http.delete<Result<void>>(`/matches/${id}`);
  }

  addLineup(matchId: string, lineup: Partial<MatchLineup>): Observable<Result<MatchLineup>> {
    return this.http.post<Result<MatchLineup>>(`/matches/${matchId}/lineups`, lineup);
  }

  removeLineup(matchId: string, lineupId: string): Observable<Result<void>> {
    return this.http.delete<Result<void>>(`/matches/${matchId}/lineups/${lineupId}`);
  }

  addGoal(matchId: string, goal: Partial<MatchGoal>): Observable<Result<MatchGoal>> {
    return this.http.post<Result<MatchGoal>>(`/matches/${matchId}/goals`, goal);
  }

  removeGoal(matchId: string, goalId: string): Observable<Result<void>> {
    return this.http.delete<Result<void>>(`/matches/${matchId}/goals/${goalId}`);
  }

  addCard(matchId: string, card: Partial<MatchCard>): Observable<Result<MatchCard>> {
    return this.http.post<Result<MatchCard>>(`/matches/${matchId}/cards`, card);
  }

  removeCard(matchId: string, cardId: string): Observable<Result<void>> {
    return this.http.delete<Result<void>>(`/matches/${matchId}/cards/${cardId}`);
  }
}
