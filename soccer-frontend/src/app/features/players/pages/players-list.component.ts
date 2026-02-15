import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { Player } from '../../../core/models';
import { PlayerService } from '../../../core/services/player.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LoadingSpinnerComponent, ConfirmationDialogComponent } from '../../../shared/components';

@Component({
  selector: 'app-players-list',
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
    <div class="players-container">
      <div class="header">
        <h1>Players</h1>
        <button mat-raised-button color="primary" routerLink="create">
          <mat-icon>add</mat-icon> Add Player
        </button>
      </div>

      <app-loading-spinner [isLoading]="isLoading" message="Loading players..."></app-loading-spinner>

      <table mat-table [dataSource]="players" class="players-table" *ngIf="!isLoading && players.length > 0">
        <ng-container matColumnDef="firstName">
          <th mat-header-cell *matHeaderCellDef>First Name</th>
          <td mat-cell *matCellDef="let element">{{ element.firstName }}</td>
        </ng-container>

        <ng-container matColumnDef="lastName">
          <th mat-header-cell *matHeaderCellDef>Last Name</th>
          <td mat-cell *matCellDef="let element">{{ element.lastName }}</td>
        </ng-container>

        <ng-container matColumnDef="jerseyNumber">
          <th mat-header-cell *matHeaderCellDef>Jersey #</th>
          <td mat-cell *matCellDef="let element">{{ element.jerseyNumber }}</td>
        </ng-container>

        <ng-container matColumnDef="position">
          <th mat-header-cell *matHeaderCellDef>Position</th>
          <td mat-cell *matCellDef="let element">{{ element.position }}</td>
        </ng-container>

        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef>Actions</th>
          <td mat-cell *matCellDef="let element">
            <button mat-icon-button routerLink="edit" [queryParams]="{ id: element.id }" title="Edit">
              <mat-icon>edit</mat-icon>
            </button>
            <button mat-icon-button color="warn" (click)="deletePlayer(element.id)" title="Delete">
              <mat-icon>delete</mat-icon>
            </button>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>

      <p *ngIf="!isLoading && players.length === 0" class="no-data">
        No players found. <a routerLink="create">Add one now</a>
      </p>
    </div>
  `,
  styles: [`
    .players-container {
      padding: 20px;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
    }

    .players-table {
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
export class PlayersListComponent implements OnInit {
  players: Player[] = [];
  isLoading = false;
  displayedColumns: string[] = ['firstName', 'lastName', 'jerseyNumber', 'position', 'actions'];

  constructor(
    private playerService: PlayerService,
    private notificationService: NotificationService,
    private dialog: MatDialog,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPlayers();
  }

  private loadPlayers(): void {
    this.isLoading = true;
    this.playerService.getPlayers().subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.players = response.data;
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notificationService.error('Failed to load players');
      }
    });
  }

  deletePlayer(id: string): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Player',
        message: 'Are you sure you want to delete this player?',
        confirmText: 'Delete'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.playerService.deletePlayer(id).subscribe({
          next: () => {
            this.notificationService.success('Player deleted successfully');
            this.loadPlayers();
          },
          error: () => {
            this.notificationService.error('Failed to delete player');
          }
        });
      }
    });
  }
}
