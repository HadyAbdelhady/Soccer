import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormArray } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router } from '@angular/router';

import { TournamentService } from '../../../core/services/tournament.service';
import { TeamService } from '../../../core/services/team.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Tournament, Team } from '../../../core/models';
import { LoadingSpinnerComponent } from '../../../shared/components';

@Component({
  selector: 'app-add-teams-to-tournament',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCheckboxModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatIconModule,
    LoadingSpinnerComponent
  ],
  template: `
    <div class="add-teams-container">
      <div class="header">
        <button mat-button (click)="goBack()" class="back-button">
          <mat-icon>arrow_back</mat-icon> Back
        </button>
        <h1>Add Teams to Tournament</h1>
      </div>

      <app-loading-spinner [isLoading]="isLoading" message="Loading data..."></app-loading-spinner>

      <div *ngIf="!isLoading && tournament" class="content">
        <mat-card class="tournament-info">
          <mat-card-header>
            <mat-card-title>{{ tournament.name }}</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <p>{{ tournament.description }}</p>
            <p><strong>Type:</strong> {{ tournament.type }}</p>
            <p><strong>Start Date:</strong> {{ tournament.startDate | date }}</p>
            <p><strong>End Date:</strong> {{ tournament.endDate | date }}</p>
          </mat-card-content>
        </mat-card>

        <form [formGroup]="teamsForm" (ngSubmit)="onSubmit()">
          <mat-card class="teams-selection">
            <mat-card-header>
              <mat-card-title>Select Teams to Add</mat-card-title>
              <mat-card-subtitle>
                {{ availableTeams.length }} teams available
              </mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              <div *ngIf="availableTeams.length === 0" class="no-teams">
                <p>No teams available to add. All teams are already in this tournament.</p>
                <button mat-raised-button color="primary" routerLink="/teams/create">
                  <mat-icon>add</mat-icon> Create New Team
                </button>
              </div>

              <div formArrayName="selectedTeams" class="teams-grid" *ngIf="availableTeams.length > 0">
                <div *ngFor="let team of availableTeams; let i = index" class="team-item">
                  <mat-checkbox 
                    [formControlName]="i"
                    [value]="team.id">
                    <div class="team-info">
                      <strong>{{ team.name }}</strong>
                      <span class="team-username">@{{ team.username }}</span>
                    </div>
                  </mat-checkbox>
                </div>
              </div>

              <div class="selection-summary" *ngIf="selectedTeamsCount > 0">
                <p>{{ selectedTeamsCount }} team(s) selected</p>
              </div>
            </mat-card-content>
            <mat-card-actions *ngIf="availableTeams.length > 0">
              <button 
                mat-raised-button 
                color="primary" 
                type="submit"
                [disabled]="selectedTeamsCount === 0 || isSubmitting">
                <mat-icon>add</mat-icon>
                {{ isSubmitting ? 'Adding...' : 'Add Selected Teams' }}
              </button>
              <button mat-button (click)="goBack()" type="button">Cancel</button>
            </mat-card-actions>
          </mat-card>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .add-teams-container {
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .header {
      display: flex;
      align-items: center;
      gap: 16px;
      margin-bottom: 30px;
    }

    .header h1 {
      margin: 0;
    }

    .back-button {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .content {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }

    .tournament-info {
      background: #f8f9fa;
    }

    .teams-selection {
      min-height: 400px;
    }

    .no-teams {
      text-align: center;
      padding: 40px;
      color: #666;
    }

    .no-teams p {
      margin-bottom: 20px;
      font-size: 16px;
    }

    .teams-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 16px;
      margin: 20px 0;
    }

    .team-item {
      padding: 12px;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      transition: all 0.2s ease;
    }

    .team-item:hover {
      border-color: #667eea;
      background-color: #f8f9ff;
    }

    .team-item mat-checkbox {
      width: 100%;
    }

    .team-info {
      display: flex;
      flex-direction: column;
      gap: 4px;
      margin-left: 8px;
    }

    .team-username {
      font-size: 12px;
      color: #666;
    }

    .selection-summary {
      margin-top: 20px;
      padding: 12px;
      background-color: #e8f5e8;
      border-radius: 4px;
      font-weight: 500;
    }

    mat-card-actions {
      display: flex;
      gap: 12px;
      padding: 16px;
    }

    @media (max-width: 768px) {
      .teams-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class AddTeamsToTournamentComponent implements OnInit {
  tournament: Tournament | null = null;
  availableTeams: Team[] = [];
  teamsForm: FormGroup;
  isLoading = false;
  isSubmitting = false;
  tournamentId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private tournamentService: TournamentService,
    private teamService: TeamService,
    private notificationService: NotificationService
  ) {
    this.teamsForm = this.fb.group({
      selectedTeams: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.tournamentId = this.route.snapshot.queryParamMap.get('id');
    if (this.tournamentId) {
      this.loadData();
    } else {
      this.notificationService.error('Tournament ID is required');
      this.goBack();
    }
  }

  get selectedTeamsArray(): FormArray {
    return this.teamsForm.get('selectedTeams') as FormArray;
  }

  get selectedTeamsCount(): number {
    return this.selectedTeamsArray.controls.filter(control => control.value).length;
  }

  private loadData(): void {
    this.isLoading = true;
    
    // Load tournament details and available teams in parallel
    this.tournamentService.getTournament(this.tournamentId!).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.tournament = response.data;
        } else {
          this.notificationService.error('Failed to load tournament');
        }
      },
      error: () => {
        this.notificationService.error('Failed to load tournament');
      }
    });

    this.teamService.getTeamsNotInTournaments().subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.availableTeams = response.data;
          this.initializeFormArray();
        } else {
          this.notificationService.error('Failed to load available teams');
        }
        this.isLoading = false;
      },
      error: () => {
        this.notificationService.error('Failed to load available teams');
        this.isLoading = false;
      }
    });
  }

  private initializeFormArray(): void {
    const array = this.selectedTeamsArray;
    array.clear();
    
    this.availableTeams.forEach(() => {
      array.push(this.fb.control(false));
    });
  }

  onSubmit(): void {
    if (this.selectedTeamsCount === 0 || !this.tournamentId) {
      return;
    }

    this.isSubmitting = true;
    
    const selectedTeamIds: string[] = [];
    this.selectedTeamsArray.controls.forEach((control, index) => {
      if (control.value && this.availableTeams[index]) {
        selectedTeamIds.push(this.availableTeams[index].id);
      }
    });

    this.tournamentService.addTeamsToTournament(this.tournamentId, selectedTeamIds).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.notificationService.success(`Successfully added ${selectedTeamIds.length} team(s) to tournament`);
          this.goBack();
        } else {
          this.notificationService.error('Failed to add teams to tournament');
        }
        this.isSubmitting = false;
      },
      error: () => {
        this.notificationService.error('Failed to add teams to tournament');
        this.isSubmitting = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/tournaments']);
  }
}
