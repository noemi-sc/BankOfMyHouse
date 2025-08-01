import { Component, OnInit } from '@angular/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { ChangeDetectionStrategy } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSidenavModule } from '@angular/material/sidenav';
import { AuthService } from '../auth/auth.service';
import { MatButtonModule } from '@angular/material/button';
import { MatGridListModule } from '@angular/material/grid-list';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  standalone: true,
  providers: [provideNativeDateAdapter()],
  imports: [
    MatSlideToggleModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    FormsModule,
    ReactiveFormsModule,
    MatSidenavModule,
    MatButtonModule,
    MatGridListModule,
    CommonModule,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent implements OnInit {
  readonly date = new FormControl(new Date());
  readonly serializedDate = new FormControl(new Date().toISOString());


  constructor(private authService: AuthService) {
    var currentUser$ = this.authService.currentUser$;
    console.log('Is logged in:', this.authService.isLoggedIn());

    // Subscribe to auth state
    this.authService.isAuthenticated$.subscribe((isAuth) => {
      console.log('Auth state changed:', isAuth);
    });
  }
  ngOnInit(): void {
    console.log('Dashboard loaded - user is authenticated');
  }
}
