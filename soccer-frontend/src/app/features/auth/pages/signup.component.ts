import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="signup-container">
      <mat-card class="signup-card">
        <mat-card-title>Create Account</mat-card-title>
        <form [formGroup]="signupForm" (ngSubmit)="onSubmit()">
          <mat-form-field appearance="fill" class="full-width">
            <mat-label>First Name</mat-label>
            <input matInput formControlName="firstName" required>
            <mat-error *ngIf="signupForm.get('firstName')?.hasError('required')">
              First name is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Last Name</mat-label>
            <input matInput formControlName="lastName" required>
            <mat-error *ngIf="signupForm.get('lastName')?.hasError('required')">
              Last name is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Email</mat-label>
            <input matInput type="email" formControlName="email" required>
            <mat-error *ngIf="signupForm.get('email')?.hasError('required')">
              Email is required
            </mat-error>
            <mat-error *ngIf="signupForm.get('email')?.hasError('email')">
              Invalid email
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Password</mat-label>
            <input matInput type="password" formControlName="password" required>
            <mat-error *ngIf="signupForm.get('password')?.hasError('required')">
              Password is required
            </mat-error>
            <mat-error *ngIf="signupForm.get('password')?.hasError('minlength')">
              Password must be at least 6 characters
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="fill" class="full-width">
            <mat-label>Confirm Password</mat-label>
            <input matInput type="password" formControlName="confirmPassword" required>
            <mat-error *ngIf="signupForm.get('confirmPassword')?.hasError('required')">
              Confirm password is required
            </mat-error>
          </mat-form-field>

          <button 
            mat-raised-button 
            color="primary" 
            type="submit" 
            class="full-width"
            [disabled]="isLoading || signupForm.invalid"
          >
            <mat-spinner *ngIf="isLoading" diameter="20"></mat-spinner>
            <span *ngIf="!isLoading">Sign Up</span>
          </button>
        </form>

        <p class="login-link">
          Already have an account? <a routerLink="/login">Login here</a>
        </p>
      </mat-card>
    </div>
  `,
  styles: [`
    .signup-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }

    .signup-card {
      width: 100%;
      max-width: 500px;
      padding: 40px;
    }

    mat-card-title {
      text-align: center;
      margin-bottom: 30px;
    }

    .full-width {
      width: 100%;
      margin-bottom: 20px;
    }

    .login-link {
      text-align: center;
      margin-top: 20px;
    }

    .login-link a {
      color: #667eea;
      text-decoration: none;
    }

    .login-link a:hover {
      text-decoration: underline;
    }

    mat-spinner {
      display: inline-block;
      margin-right: 10px;
    }
  `]
})
export class SignupComponent implements OnInit {
  signupForm!: FormGroup;
  isLoading = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.signupForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.signupForm.invalid) {
      return;
    }

    const password = this.signupForm.get('password')?.value;
    const confirmPassword = this.signupForm.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      this.notificationService.error('Passwords do not match');
      return;
    }

    this.isLoading = true;
    const userData = {
      firstName: this.signupForm.get('firstName')?.value,
      lastName: this.signupForm.get('lastName')?.value,
      email: this.signupForm.get('email')?.value,
      password: password
    };

    this.authService.register(userData).subscribe({
      next: () => {
        this.notificationService.success('Account created successfully!');
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.isLoading = false;
        this.notificationService.error('Registration failed. Please try again.');
      }
    });
  }
}
