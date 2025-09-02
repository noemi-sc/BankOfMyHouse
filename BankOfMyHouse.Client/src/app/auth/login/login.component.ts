import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { UserLoginRequestDto as UserLoginRequestDto } from '../models/auth-response';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FontAwesomeModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})

export class LoginComponent implements OnInit {
  /*   onForgotPassword($event: MouseEvent) {
      throw new Error('Method not implemented.');
    } */

  protected readonly faEye = faEye;
  protected readonly faEyeSlash = faEyeSlash;
  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
  loginForm!: FormGroup;
  submitted = false;
  loading = false;
  error = '';
  showPassword: boolean = false;
  success: boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
  ) { }

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
    this.submitted = true;
    this.error = '';

    // Stop if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;

    // Prepare user data (exclude confirmPassword)
    const userData: UserLoginRequestDto = {
      username: this.f['username'].value,
      password: this.f['password'].value,
    };

    this.authService.login(userData).subscribe({

      next: (response) => {
        this.loading = false;
        this.success = true;

        // Optional: Auto-login after registration
        // Or redirect to login page
        setTimeout(() => {
          this.router.navigate(['/home']);
        }, 2000);
      },
      error: (error) => {
        this.loading = false;
        this.error =
          error.error?.message || 'Login failed. Please try again.';
      },
    });
  }
}
