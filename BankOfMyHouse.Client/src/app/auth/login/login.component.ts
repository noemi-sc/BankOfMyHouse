import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserLoginRequestDto as UserLoginRequestDto } from '../models/auth-response';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';
import { UserService } from '../../services/users/users.service';

@Component({
  selector: 'app-login',  
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FontAwesomeModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})

export class LoginComponent implements OnInit {

  protected readonly faEye = faEye;
  protected readonly faEyeSlash = faEyeSlash;
  protected error = signal<string>('');

  private formBuilder: FormBuilder = inject(FormBuilder);
  private userService: UserService = inject(UserService);
  private router: Router = inject(Router);

  protected loginForm!: FormGroup;
  protected submitted = signal<boolean>(false);
  loading = this.userService.loading;

  protected showPassword: boolean = false;
  private success = signal<boolean>(false);

  get f() {
    return this.loginForm.controls;
  }

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    this.submitted.set(true);
    this.error.set('');

    // Stop if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    // Prepare user data (exclude confirmPassword)
    const userData: UserLoginRequestDto = {
      username: this.f['username'].value,
      password: this.f['password'].value,
    };

    this.userService.login(userData).subscribe({

      next: (response) => {
        this.success.set(true);

        // Optional: Auto-login after registration
        // Or redirect to login page
        setTimeout(() => {
          this.router.navigate(['/home']);
        }, 2000);
      },
      error: (error) => {
        this.error = error.error?.message || 'Login failed. Please try again.';
      },
    });
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
}
