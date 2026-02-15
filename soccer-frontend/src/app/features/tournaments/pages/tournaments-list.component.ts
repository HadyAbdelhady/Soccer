import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Tournament } from '../../../core/models';
import { TournamentService } from '../../../core/services/tournament.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LoadingSpinnerComponent } from '../../../shared/components';

@Component({
  selector: 'app-tournaments-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    LoadingSpinnerComponent
  ],
  template: `
    <div class="tournaments-container">
      <div class="header">
        <h1>Tournaments</h1>
        <button mat-raised-button color="primary" routerLink="create">
          <mat-icon>add</mat-icon> Create Tournament
        </button>
      </div>

      <app-loading-spinner [isLoading]="isLoading" message="Loading tournaments..."></app-loading-spinner>

      <div *ngIf="!isLoading && tournaments.length > 0" class="tournaments-grid">
        <mat-card *ngFor="let tournament of tournaments" class="tournament-card">
          <mat-card-header>
            <mat-card-title>{{ tournament.name }}</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <p>{{ tournament.description }}</p>
            <p class="type">Type: {{ tournament.type }}</p>
            <p>Start: {{ tournament.startDate | date }}</p>
            <p>End: {{ tournament.endDate | date }}</p>
          </mat-card-content>
          <mat-card-actions>
            <button mat-button routerLink="detail" [queryParams]="{ id: tournament.id }">View Details</button>
            <button mat-button routerLink="edit" [queryParams]="{ id: tournament.id }">Edit</button>
          </mat-card-actions>
        </mat-card>
      </div>

      <p *ngIf="!isLoading && tournaments.length === 0" class="no-data">
        No tournaments found. <a routerLink="create">Create one now</a>
      </p>
    </div>
  `,
  styles: [`
    .tournaments-container {
      padding: 20px;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
    }

    .tournaments-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 20px;
    }

    .tournament-card {
      cursor: pointer;
    }

    .tournament-card:hover {
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
    }

    .type {
      font-weight: bold;
      color: #667eea;
    }

    .no-data {
      text-align: center;
      padding: 40px;
      color: #999;
    }

    .no-data a {
      color: #667eea;
      text-decoration: none;
    }
  `]
})
export class TournamentsListComponent implements OnInit {
  tournaments: Tournament[] = [];
  isLoading = false;

  constructor(
    private tournamentService: TournamentService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadTournaments();
  }

  private loadTournaments(): void {
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
        this.notificationService.error('Failed to load tournaments');
      }
    });
  }
}
