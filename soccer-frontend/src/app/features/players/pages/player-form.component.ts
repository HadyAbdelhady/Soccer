import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { Player, PlayerPosition } from '../../../core/models';
import { PlayerService } from '../../../core/services/player.service';
import { TeamService } from '../../../core/services/team.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LoadingSpinnerComponent } from '../../../shared/components';

@Component({
  selector: 'app-player-form',
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
    <div class="player-form-container">
      <mat-card class="form-card">
        <mat-card-title>{{ isEditMode ? 'Edit Player' : 'Add New Player' }}</mat-card-title>
        
        <app-loading-spinner [isLoading]="isLoading" message="Loading..."></app-loading-spinner>

        <form [formGroup]="playerForm" (ngSubmit)="onSubmit()" *ngIf="!isLoading">
          <mat-form-field appearance="fill" class="full-width">
            <mat-label>First Name</mat-label>
            <input matInput formControlName="firstName" required>
            <mat-error *ngIf="playerForm.get('firstName')?.hasError('required')">
              First name is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Last Name</mat-label>
            <input matInput formControlName="lastName" required>
            <mat-error *ngIf="playerForm.get('lastName')?.hasError('required')">
              Last name is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Jersey Number</mat-label>
            <input matInput type="number" formControlName="jerseyNumber" required>
            <mat-error *ngIf="playerForm.get('jerseyNumber')?.hasError('required')">
              Jersey number is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Position</mat-label>
            <mat-select formControlName="position" required>
              <mat-option *ngFor="let pos of positions" [value]="pos">{{ pos }}</mat-option>
            </mat-select>
            <mat-error *ngIf="playerForm.get('position')?.hasError('required')">
              Position is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Team ID</mat-label>
            <input matInput formControlName="teamId" required>
            <mat-error *ngIf="playerForm.get('teamId')?.hasError('required')">
              Team is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Date of Birth (optional)</mat-label>
            <input matInput type="date" formControlName="dateOfBirth">
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Nationality (optional)</mat-label>
            <input matInput formControlName="nationality">
          </mat-form-field>

          <div class="button-group">
            <button mat-raised-button color="primary" type="submit" [disabled]="playerForm.invalid">
              {{ isEditMode ? 'Update' : 'Create' }}
            </button>
            <button mat-button type="button" (click)="goBack()">Cancel</button>
          </div>
        </form>
      </mat-card>
    </div>
  `,
  styles: [`
    .player-form-container {
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
  `]
})
export class PlayerFormComponent implements OnInit {
  playerForm!: FormGroup;
  isLoading = false;
  isEditMode = false;
  playerId?: string;
  positions = Object.values(PlayerPosition);

  constructor(
    private formBuilder: FormBuilder,
    private playerService: PlayerService,
    private teamService: TeamService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.route.queryParams.subscribe(params => {
      if (params['id']) {
        this.playerId = params['id'];
        this.isEditMode = true;
        this.loadPlayer();
      }
    });
  }

  private initializeForm(): void {
    this.playerForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      jerseyNumber: ['', Validators.required],
      position: ['', Validators.required],
      teamId: ['', Validators.required],
      dateOfBirth: [''],
      nationality: ['']
    });
  }

  private loadPlayer(): void {
    if (!this.playerId) return;

    this.isLoading = true;
    this.playerService.getPlayer(this.playerId).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.playerForm.patchValue(response.data);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notificationService.error('Failed to load player');
        this.goBack();
      }
    });
  }

  onSubmit(): void {
    if (this.playerForm.invalid) {
      return;
    }

    const playerData = this.playerForm.value;
    const request = this.isEditMode && this.playerId
      ? this.playerService.updatePlayer(this.playerId, playerData)
      : this.playerService.createPlayer(playerData);

    request.subscribe({
      next: () => {
        this.notificationService.success(
          this.isEditMode ? 'Player updated successfully' : 'Player created successfully'
        );
        this.router.navigate(['/players']);
      },
      error: () => {
        this.notificationService.error('Failed to save player');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/players']);
  }
}
