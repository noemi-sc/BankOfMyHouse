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

  private formBuilder: FormBuilder = inject(FormBuilder);
  private userService: UserService = inject(UserService);
  private router: Router = inject(Router);

  protected loginForm!: FormGroup;
  loading = this.userService.loading;

  protected showPassword = signal<boolean>(false);
  protected submitted = signal<boolean>(false);
  protected error = signal<string>('');
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
        if (response.user !== null) {
          this.success.set(true);
          this.router.navigate(['/home']);
        }
      },
      error: (error) => {
        this.error.set(error.error?.message || 'Login failed. Please try again.');
      },
    });
  }

  togglePasswordVisibility() {
    this.showPassword.set(!this.showPassword());
  }
}
