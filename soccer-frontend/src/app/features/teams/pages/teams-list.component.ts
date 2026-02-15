import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { Team } from '../../../core/models';
import { TeamService } from '../../../core/services/team.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LoadingSpinnerComponent, ConfirmationDialogComponent } from '../../../shared/components';

@Component({
  selector: 'app-teams-list',
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
    <div class="teams-container">
      <div class="header">
        <h1>Teams</h1>
        <button mat-raised-button color="primary" routerLink="create">
          <mat-icon>add</mat-icon> Create Team
        </button>
      </div>

      <app-loading-spinner [isLoading]="isLoading" message="Loading teams..."></app-loading-spinner>

      <table mat-table [dataSource]="teams" class="teams-table" *ngIf="!isLoading && teams.length > 0">
        <ng-container matColumnDef="name">
          <th mat-header-cell *matHeaderCellDef>Team Name</th>
          <td mat-cell *matCellDef="let element">{{ element.name }}</td>
        </ng-container>

        <ng-container matColumnDef="description">
          <th mat-header-cell *matHeaderCellDef>Description</th>
          <td mat-cell *matCellDef="let element">{{ element.description }}</td>
        </ng-container>

        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef>Actions</th>
          <td mat-cell *matCellDef="let element">
            <button mat-icon-button routerLink="edit" [queryParams]="{ id: element.id }" title="Edit">
              <mat-icon>edit</mat-icon>
            </button>
            <button mat-icon-button color="warn" (click)="deleteTeam(element.id)" title="Delete">
              <mat-icon>delete</mat-icon>
            </button>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>

      <p *ngIf="!isLoading && teams.length === 0" class="no-data">
        No teams found. <a routerLink="create">Create one now</a>
      </p>
    </div>
  `,
  styles: [`
    .teams-container {
      padding: 20px;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
    }

    .teams-table {
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
export class TeamsListComponent implements OnInit {
  teams: Team[] = [];
  isLoading = false;
  displayedColumns: string[] = ['name', 'description', 'actions'];

  constructor(
    private teamService: TeamService,
    private notificationService: NotificationService,
    private dialog: MatDialog,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTeams();
  }

  private loadTeams(): void {
    this.isLoading = true;
    this.teamService.getTeams().subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.teams = response.data;
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notificationService.error('Failed to load teams');
      }
    });
  }

  deleteTeam(id: string): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Team',
        message: 'Are you sure you want to delete this team?',
        confirmText: 'Delete'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.teamService.deleteTeam(id).subscribe({
          next: () => {
            this.notificationService.success('Team deleted successfully');
            this.loadTeams();
          },
          error: () => {
            this.notificationService.error('Failed to delete team');
          }
        });
      }
    });
  }
}
