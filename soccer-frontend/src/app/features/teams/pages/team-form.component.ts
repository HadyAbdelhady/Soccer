import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Team } from '../../../core/models';
import { TeamService } from '../../../core/services/team.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LoadingSpinnerComponent } from '../../../shared/components';

@Component({
  selector: 'app-team-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    LoadingSpinnerComponent
  ],
  template: `
    <div class="team-form-container">
      <mat-card class="form-card">
        <mat-card-title>{{ isEditMode ? 'Edit Team' : 'Create New Team' }}</mat-card-title>
        
        <app-loading-spinner [isLoading]="isLoading" message="Loading..."></app-loading-spinner>

        <form [formGroup]="teamForm" (ngSubmit)="onSubmit()" *ngIf="!isLoading">
          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Team Name</mat-label>
            <input matInput formControlName="name" required>
            <mat-error *ngIf="teamForm.get('name')?.hasError('required')">
              Team name is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Description</mat-label>
            <textarea matInput formControlName="description" required></textarea>
            <mat-error *ngIf="teamForm.get('description')?.hasError('required')">
              Description is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Logo URL (optional)</mat-label>
            <input matInput formControlName="logoUrl">
          </mat-form-field>

          <div class="button-group">
            <button mat-raised-button color="primary" type="submit" [disabled]="teamForm.invalid">
              {{ isEditMode ? 'Update' : 'Create' }}
            </button>
            <button mat-button type="button" (click)="goBack()">Cancel</button>
          </div>
        </form>
      </mat-card>
    </div>
  `,
  styles: [`
    .team-form-container {
      padding: 20px;
    }

    .form-card {
      max-width: 600px;
      margin: 0 auto;
      padding: 30px;
    }

    mat-card-title {
      margin-bottom: 30px;
    }

    .full-width {
      width: 100%;
      margin-bottom: 20px;
    }

    .button-group {
      display: flex;
      gap: 10px;
      margin-top: 30px;
    }

    textarea {
      resize: vertical;
      min-height: 100px;
    }
  `]
})
export class TeamFormComponent implements OnInit {
  teamForm!: FormGroup;
  isLoading = false;
  isEditMode = false;
  teamId?: string;

  constructor(
    private formBuilder: FormBuilder,
    private teamService: TeamService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.route.queryParams.subscribe(params => {
      if (params['id']) {
        this.teamId = params['id'];
        this.isEditMode = true;
        this.loadTeam();
      }
    });
  }

  private initializeForm(): void {
    this.teamForm = this.formBuilder.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      logoUrl: ['']
    });
  }

  private loadTeam(): void {
    if (!this.teamId) return;

    this.isLoading = true;
    this.teamService.getTeam(this.teamId).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.teamForm.patchValue(response.data);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notificationService.error('Failed to load team');
        this.goBack();
      }
    });
  }

  onSubmit(): void {
    if (this.teamForm.invalid) {
      return;
    }

    const teamData = this.teamForm.value;
    const request = this.isEditMode && this.teamId
      ? this.teamService.updateTeam(this.teamId, teamData)
      : this.teamService.createTeam(teamData);

    request.subscribe({
      next: () => {
        this.notificationService.success(
          this.isEditMode ? 'Team updated successfully' : 'Team created successfully'
        );
        this.router.navigate(['/teams']);
      },
      error: () => {
        this.notificationService.error('Failed to save team');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/teams']);
  }
}
