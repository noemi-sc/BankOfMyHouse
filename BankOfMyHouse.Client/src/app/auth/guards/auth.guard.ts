import { Injectable, inject } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { UserService } from '../../services/users/users.service';

@Injectable({
  providedIn: 'root'
})

export class AuthGuard implements CanActivate {
  private authService: UserService = inject(UserService);
  private router: Router = inject(Router);

  canActivate(): boolean {
    const isAuthenticated = this.authService.isAuthenticated();
    
    if (isAuthenticated) {
      return true;
    } else {
      this.router.navigate(['/login']);
      return false;
    }
  }
}
