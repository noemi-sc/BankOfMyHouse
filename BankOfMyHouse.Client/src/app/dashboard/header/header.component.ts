// advanced-header.component.ts
import { Component, ChangeDetectionStrategy, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { NgOptimizedImage } from '@angular/common';
import { UserService } from '../../services/users/users.service';
import { faUserCircle } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterModule, NgOptimizedImage, FontAwesomeModule],
  host: {
    '(document:click)': 'onDocumentClick($event)'
  }
})
export class HeaderComponent implements OnInit {

  protected readonly faUserCircle = faUserCircle;

  private usersService = inject(UserService);
  private router: Router = inject(Router);

  // State signals -- da mettere current user
  currentUser = this.usersService.userDetails;
  loading = this.usersService.loading;
  error = this.usersService.error;

  ngOnInit() {
    this.usersService.getUserDetails();
  }

  mobileMenuOpen = signal(false);
  notificationsOpen = signal(false);
  userMenuOpen = signal(false);

  navItems = signal([
    // { label: 'Dashboard', path: '/dashboard', icon: 'icon-dashboard', highlight: false },
    { label: 'Conti bancari', path: '/home', icon: 'icon-wallet', highlight: false },
/*     { label: 'Payments', path: '/home', icon: 'icon-credit-card', highlight: true },
 */    { label: 'Investimenti', path: '/investments', icon: 'icon-trending-up', highlight: false }
  ]);

  // Event handlers
  toggleMobileMenu() {
    this.mobileMenuOpen.update(open => !open);
    this.notificationsOpen.set(false);
    this.userMenuOpen.set(false);
  }

  closeMobileMenu() {
    this.mobileMenuOpen.set(false);
  }

  toggleUserMenu() {
    this.userMenuOpen.update(open => !open);
    this.notificationsOpen.set(false);
    this.mobileMenuOpen.set(false);
  }


  onDocumentClick(event: Event) {
    const target = event.target as Element;
    if (!target.closest('.notifications-wrapper')) {
      this.notificationsOpen.set(false);
    }
    if (!target.closest('.user-menu-wrapper')) {
      this.userMenuOpen.set(false);
    }
    if (!target.closest('.main-nav') && !target.closest('.mobile-menu-toggle')) {
      this.mobileMenuOpen.set(false);
    }
  }

  formatTime(timestamp: Date): string {
    const now = new Date();
    const diff = now.getTime() - timestamp.getTime();
    const hours = Math.floor(diff / (1000 * 60 * 60));

    if (hours < 1) return 'Just now';
    if (hours < 24) return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    return `${days}d ago`;
  }

  onLogout() {
    console.log('Logging out...');

    this.usersService.clearAuthData();
    this.router.navigate(['/login']);
  }
}
