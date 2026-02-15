import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';
import { Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          this.authService.logout();
          this.router.navigate(['/login']);
          this.notificationService.error('Your session has expired. Please login again.');
        } else if (error.status === 403) {
          this.notificationService.error('You do not have permission to access this resource.');
        } else if (error.status === 400) {
          const errorMessage = error.error?.errorMessage || 'Invalid request';
          this.notificationService.error(errorMessage);
        } else if (error.status === 404) {
          this.notificationService.error('Resource not found');
        } else if (error.status === 500) {
          this.notificationService.error('An internal server error occurred');
        } else if (error.status === 0) {
          this.notificationService.error('Unable to connect to the server');
        }
        return throwError(() => error);
      })
    );
  }
}
