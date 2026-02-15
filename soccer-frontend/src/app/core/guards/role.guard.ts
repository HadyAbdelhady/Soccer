import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const requiredRoles: UserRole[] = route.data['roles'] || [];

    if (requiredRoles.length === 0) {
      return true;
    }

    const currentUser = this.authService.getCurrentUser();
    if (!currentUser || !requiredRoles.includes(currentUser.role)) {
      this.router.navigate(['/dashboard']);
      return false;
    }

    return true;
  }
}
