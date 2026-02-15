import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../core/services';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, MatToolbarModule, MatButtonModule, MatMenuModule, MatIconModule, MatDividerModule],
  template: `
    <mat-toolbar color="primary">
      <h1 class="toolbar-title">Soccer Manager</h1>
      <span class="spacer"></span>
      <button mat-icon-button [matMenuTriggerFor]="menu" *ngIf="isAuthenticated$ | async">
        <mat-icon>account_circle</mat-icon>
      </button>
      <mat-menu #menu="matMenu">
        <button mat-menu-item routerLink="/dashboard">Dashboard</button>
        <button mat-menu-item routerLink="/teams">Teams</button>
        <button mat-menu-item routerLink="/players">Players</button>
        <button mat-menu-item routerLink="/tournaments">Tournaments</button>
        <button mat-menu-item routerLink="/matches">Matches</button>
        <mat-divider></mat-divider>
        <button mat-menu-item (click)="logout()">Logout</button>
      </mat-menu>
    </mat-toolbar>
  `,
  styles: [`
    .spacer {
      flex: 1 1 auto;
    }
    .toolbar-title {
      margin: 0;
      font-size: 20px;
    }
  `]
})
export class HeaderComponent {
  isAuthenticated$ = this.authService.isAuthenticated$;

  constructor(private authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }
}
