import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { TournamentService } from '../../../core/services/tournament.service';
import { MatchService } from '../../../core/services/match.service';
import { Tournament, Match } from '../../../core/models';
import { LoadingSpinnerComponent } from '../../../shared/components';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatGridListModule,
    MatButtonModule,
    RouterModule,
    LoadingSpinnerComponent
  ],
  template: `
    <div class="dashboard-container">
      <h1>Dashboard</h1>
      
      <mat-grid-list cols="3" rowHeight="200px" [gutterSize]="'20'">
        <!-- Tournaments Card -->
        <mat-grid-tile>
          <mat-card class="dashboard-card">
            <mat-card-title>Tournaments</mat-card-title>
            <mat-card-content>
              <p class="stat-number">{{ tournaments.length }}</p>
              <p>Active Tournaments</p>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button routerLink="/tournaments">View All</button>
            </mat-card-actions>
          </mat-card>
        </mat-grid-tile>

        <!-- Matches Card -->
        <mat-grid-tile>
          <mat-card class="dashboard-card">
            <mat-card-title>Matches</mat-card-title>
            <mat-card-content>
              <p class="stat-number">{{ matches.length }}</p>
              <p>Upcoming Matches</p>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button routerLink="/matches">View All</button>
            </mat-card-actions>
          </mat-card>
        </mat-grid-tile>

        <!-- Quick Actions -->
        <mat-grid-tile>
          <mat-card class="dashboard-card">
            <mat-card-title>Quick Actions</mat-card-title>
            <mat-card-content>
              <p>Manage your tournaments and matches</p>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button routerLink="/teams">Teams</button>
              <button mat-button routerLink="/players">Players</button>
            </mat-card-actions>
          </mat-card>
        </mat-grid-tile>
      </mat-grid-list>

      <app-loading-spinner [isLoading]="isLoading" message="Loading dashboard data..."></app-loading-spinner>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 20px;
    }

    h1 {
      margin-bottom: 30px;
      color: #333;
    }

    .dashboard-card {
      width: 100%;
      height: 100%;
      display: flex;
      flex-direction: column;
    }

    .stat-number {
      font-size: 32px;
      font-weight: bold;
      color: #667eea;
      margin: 10px 0;
    }

    mat-card-content {
      flex: 1;
      display: flex;
      flex-direction: column;
      justify-content: center;
    }

    mat-card-actions {
      display: flex;
      gap: 10px;
    }
  `]
})
export class DashboardComponent implements OnInit {
  tournaments: Tournament[] = [];
  matches: Match[] = [];
  isLoading = false;

  constructor(
    private tournamentService: TournamentService,
    private matchService: MatchService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.isLoading = true;
    this.tournamentService.getTournaments().subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.tournaments = response.data;
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });

    this.matchService.getMatches().subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.matches = response.data;
        }
      }
    });
  }
}
