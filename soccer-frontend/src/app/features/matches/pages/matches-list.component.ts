import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Match, MatchStatus } from '../../../core/models';
import { MatchService } from '../../../core/services/match.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LoadingSpinnerComponent } from '../../../shared/components';

@Component({
  selector: 'app-matches-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    LoadingSpinnerComponent
  ],
  template: `
    <div class="matches-container">
      <div class="header">
        <h1>Matches</h1>
        <button mat-raised-button color="primary" routerLink="create">
          <mat-icon>add</mat-icon> Create Match
        </button>
      </div>

      <app-loading-spinner [isLoading]="isLoading" message="Loading matches..."></app-loading-spinner>

      <table mat-table [dataSource]="matches" class="matches-table" *ngIf="!isLoading && matches.length > 0">
        <ng-container matColumnDef="homeTeam">
          <th mat-header-cell *matHeaderCellDef>Home Team</th>
          <td mat-cell *matCellDef="let element">{{ element.homeTeamName }}</td>
        </ng-container>

        <ng-container matColumnDef="score">
          <th mat-header-cell *matHeaderCellDef>Score</th>
          <td mat-cell *matCellDef="let element">{{ element.homeTeamGoals }} - {{ element.awayTeamGoals }}</td>
        </ng-container>

        <ng-container matColumnDef="awayTeam">
          <th mat-header-cell *matHeaderCellDef>Away Team</th>
          <td mat-cell *matCellDef="let element">{{ element.awayTeamName }}</td>
        </ng-container>

        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Status</th>
          <td mat-cell *matCellDef="let element">{{ element.status }}</td>
        </ng-container>

        <ng-container matColumnDef="date">
          <th mat-header-cell *matHeaderCellDef>Date</th>
          <td mat-cell *matCellDef="let element">{{ element.matchDate | date: 'short' }}</td>
        </ng-container>

        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef>Actions</th>
          <td mat-cell *matCellDef="let element">
            <button mat-icon-button routerLink="detail" [queryParams]="{ id: element.id }" title="View Details">
              <mat-icon>visibility</mat-icon>
            </button>
            <button mat-icon-button routerLink="edit" [queryParams]="{ id: element.id }" title="Edit">
              <mat-icon>edit</mat-icon>
            </button>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>

      <p *ngIf="!isLoading && matches.length === 0" class="no-data">
        No matches found. <a routerLink="create">Create one now</a>
      </p>
    </div>
  `,
  styles: [`
    .matches-container {
      padding: 20px;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
    }

    .matches-table {
      width: 100%;
      margin-top: 20px;
      border-collapse: collapse;
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
export class MatchesListComponent implements OnInit {
  matches: Match[] = [];
  isLoading = false;
  displayedColumns: string[] = ['homeTeam', 'score', 'awayTeam', 'status', 'date', 'actions'];

  constructor(
    private matchService: MatchService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadMatches();
  }

  private loadMatches(): void {
    this.isLoading = true;
    this.matchService.getMatches().subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.matches = response.data;
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notificationService.error('Failed to load matches');
      }
    });
  }
}
