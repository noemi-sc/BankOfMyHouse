import { Injectable, inject } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { UserService } from '../../services/users/users.service';

@Injectable({
  providedIn: 'root'
})
export class GuestGuard implements CanActivate {

  private authService = inject(UserService);
  private router = inject(Router);

  canActivate(): boolean {

    const isAuthenticated = this.authService.isAuthenticated();

    if (!isAuthenticated) {
      return true; // Allow access to login/register
    } else {
      this.router.navigate(['/home']); // Redirect authenticated users
      return false;
    }
  }
}