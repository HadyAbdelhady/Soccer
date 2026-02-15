import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { Tournament, TournamentType } from '../../../core/models';
import { TournamentService } from '../../../core/services/tournament.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LoadingSpinnerComponent } from '../../../shared/components';

@Component({
  selector: 'app-tournament-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    LoadingSpinnerComponent
  ],
  template: `
    <div class="tournament-form-container">
      <mat-card class="form-card">
        <mat-card-title>{{ isEditMode ? 'Edit Tournament' : 'Create New Tournament' }}</mat-card-title>
        
        <app-loading-spinner [isLoading]="isLoading" message="Loading..."></app-loading-spinner>

        <form [formGroup]="tournamentForm" (ngSubmit)="onSubmit()" *ngIf="!isLoading">
          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Tournament Name</mat-label>
            <input matInput formControlName="name" required>
            <mat-error *ngIf="tournamentForm.get('name')?.hasError('required')">
              Tournament name is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Description</mat-label>
            <textarea matInput formControlName="description" required></textarea>
            <mat-error *ngIf="tournamentForm.get('description')?.hasError('required')">
              Description is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Type</mat-label>
            <mat-select formControlName="type" required>
              <mat-option *ngFor="let type of tournamentTypes" [value]="type">{{ type }}</mat-option>
            </mat-select>
            <mat-error *ngIf="tournamentForm.get('type')?.hasError('required')">
              Type is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Start Date</mat-label>
            <input matInput type="date" formControlName="startDate" required>
            <mat-error *ngIf="tournamentForm.get('startDate')?.hasError('required')">
              Start date is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>End Date</mat-label>
            <input matInput type="date" formControlName="endDate" required>
            <mat-error *ngIf="tournamentForm.get('endDate')?.hasError('required')">
              End date is required
            </mat-error>
          </mat-form-field>

          <div class="button-group">
            <button mat-raised-button color="primary" type="submit" [disabled]="tournamentForm.invalid">
              {{ isEditMode ? 'Update' : 'Create' }}
            </button>
            <button mat-button type="button" (click)="goBack()">Cancel</button>
          </div>
        </form>
      </mat-card>
    </div>
  `,
  styles: [`
    .tournament-form-container {
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
export class TournamentFormComponent implements OnInit {
  tournamentForm!: FormGroup;
  isLoading = false;
  isEditMode = false;
  tournamentId?: string;
  tournamentTypes = Object.values(TournamentType);

  constructor(
    private formBuilder: FormBuilder,
    private tournamentService: TournamentService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.route.queryParams.subscribe(params => {
      if (params['id']) {
        this.tournamentId = params['id'];
        this.isEditMode = true;
        this.loadTournament();
      }
    });
  }

  private initializeForm(): void {
    this.tournamentForm = this.formBuilder.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      type: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required]
    });
  }

  private loadTournament(): void {
    if (!this.tournamentId) return;

    this.isLoading = true;
    this.tournamentService.getTournament(this.tournamentId).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.tournamentForm.patchValue(response.data);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notificationService.error('Failed to load tournament');
        this.goBack();
      }
    });
  }

  onSubmit(): void {
    if (this.tournamentForm.invalid) {
      return;
    }

    const tournamentData = this.tournamentForm.value;
    const request = this.isEditMode && this.tournamentId
      ? this.tournamentService.updateTournament(this.tournamentId, tournamentData)
      : this.tournamentService.createTournament(tournamentData);

    request.subscribe({
      next: () => {
        this.notificationService.success(
          this.isEditMode ? 'Tournament updated successfully' : 'Tournament created successfully'
        );
        this.router.navigate(['/tournaments']);
      },
      error: () => {
        this.notificationService.error('Failed to save tournament');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/tournaments']);
  }
}
