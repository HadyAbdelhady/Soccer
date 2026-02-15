import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { NotificationService, Notification } from '../../core/services/notification.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, MatSnackBarModule],
  template: `
    <div class="notifications-container">
      <div 
        *ngFor="let notification of notifications" 
        [class]="'notification notification-' + notification.type"
        (click)="removeNotification(notification.id)"
      >
        {{ notification.message }}
      </div>
    </div>
  `,
  styles: [`
    .notifications-container {
      position: fixed;
      top: 80px;
      right: 20px;
      z-index: 9999;
      max-width: 400px;
    }

    .notification {
      padding: 16px;
      margin-bottom: 10px;
      border-radius: 4px;
      cursor: pointer;
      animation: slideIn 0.3s ease-in-out;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
    }

    .notification-success {
      background-color: #4caf50;
      color: white;
    }

    .notification-error {
      background-color: #f44336;
      color: white;
    }

    .notification-warning {
      background-color: #ff9800;
      color: white;
    }

    .notification-info {
      background-color: #2196f3;
      color: white;
    }

    @keyframes slideIn {
      from {
        transform: translateX(400px);
        opacity: 0;
      }
      to {
        transform: translateX(0);
        opacity: 1;
      }
    }
  `]
})
export class NotificationsComponent implements OnInit, OnDestroy {
  notifications: Notification[] = [];
  private destroy$ = new Subject<void>();

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.notificationService.notifications$
      .pipe(takeUntil(this.destroy$))
      .subscribe(notifications => {
        this.notifications = notifications;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  removeNotification(id: string): void {
    this.notificationService.remove(id);
  }
}
