// advanced-header.component.ts
import { Component, ChangeDetectionStrategy, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { NgOptimizedImage } from '@angular/common';
import { UserDto } from '../../auth/models/user';
import { AuthService } from '../../auth/auth.service';

interface Notification {
  id: string;
  message: string;
  type: 'info' | 'warning' | 'success';
  timestamp: Date;
  read: boolean;
}

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterModule, NgOptimizedImage],
  host: {
    '(document:click)': 'onDocumentClick($event)'
  }
})
export class HeaderComponent {
  // State signals
  currentUser = signal<UserDto | null>({
    email: 'John Doe',
    username: '1234567890',
    createdAt: '2023-10-01T12:00:00Z',
    isActive: true,
    roles: ['User'],

  });

  constructor(private router: Router, private authService: AuthService) { }
  mobileMenuOpen = signal(false);
  notificationsOpen = signal(false);
  userMenuOpen = signal(false);

  notifications = signal<Notification[]>([
    {
      id: '1',
      message: 'Your monthly statement is now available',
      type: 'info',
      timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000),
      read: false
    },
    {
      id: '2',
      message: 'Security alert: New device login detected',
      type: 'warning',
      timestamp: new Date(Date.now() - 24 * 60 * 60 * 1000),
      read: false
    },
    {
      id: '3',
      message: 'Transfer to John Smith completed successfully',
      type: 'success',
      timestamp: new Date(Date.now() - 48 * 60 * 60 * 1000),
      read: true
    }
  ]);

  navItems = signal([
    // { label: 'Dashboard', path: '/dashboard', icon: 'icon-dashboard', highlight: false },
    { label: 'Accounts', path: '/accounts', icon: 'icon-wallet', highlight: false },
    { label: 'Transfers', path: '/transfers', icon: 'icon-exchange', highlight: false },
    { label: 'Payments', path: '/payments', icon: 'icon-credit-card', highlight: true },
    { label: 'Investments', path: '/investments', icon: 'icon-trending-up', highlight: false },
    { label: 'Support', path: '/support', icon: 'icon-help', highlight: false }
  ]);

  // Computed values
  unreadCount = computed(() =>
    this.notifications().filter(n => !n.read).length
  );

  // Event handlers
  toggleMobileMenu() {
    this.mobileMenuOpen.update(open => !open);
    this.notificationsOpen.set(false);
    this.userMenuOpen.set(false);
  }

  closeMobileMenu() {
    this.mobileMenuOpen.set(false);
  }

  toggleNotifications() {
    this.notificationsOpen.update(open => !open);
    this.userMenuOpen.set(false);
    this.mobileMenuOpen.set(false);
  }

  toggleUserMenu() {
    this.userMenuOpen.update(open => !open);
    this.notificationsOpen.set(false);
    this.mobileMenuOpen.set(false);
  }

  markAsRead(notificationId: string) {
    this.notifications.update(notifications =>
      notifications.map(n =>
        n.id === notificationId ? { ...n, read: true } : n
      )
    );
  }

  markAllAsRead() {
    this.notifications.update(notifications =>
      notifications.map(n => ({ ...n, read: true }))
    );
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

    // Optional: Call logout API
    this.authService.logout();
    this.clearUserData();
    this.router.navigate(['/login']);
  }

  private clearUserData() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }
}
