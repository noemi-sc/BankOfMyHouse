import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { RegisterUserRequestDto } from '../models/auth-response';
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { UserService } from '../../services/users/users.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink, FontAwesomeModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {

  protected readonly faEye = faEye;
  protected readonly faEyeSlash = faEyeSlash;

  private formBuilder: FormBuilder = inject(FormBuilder);
  private userService: UserService = inject(UserService);
  private router: Router = inject(Router);

  registerForm!: FormGroup;
  submitted = false;
  loading = false;
  error = '';
  showPassword: boolean = false;
  success = false;

  ngOnInit(): void {
    this.registerForm = this.formBuilder.group(
      {
        username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', [Validators.required]],
        firstname: ['', [Validators.required, Validators.maxLength(50)]],
        lastname: ['', [Validators.required, Validators.maxLength(50)]],
      },
      {
        validators: this.passwordMatchValidator, // Custom validator for password matching
      }
    );
  }

  // Custom validator to check if passwords match
  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (
      password &&
      confirmPassword &&
      password.value !== confirmPassword.value
    ) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return;
  }

  // Getter for easy access to form controls in template
  get f() {
    return this.registerForm.controls;
  }

  onSubmit(): void {
    this.submitted = true;
    this.error = '';

    // Stop if form is invalid
    if (this.registerForm.invalid) {
      return;
    }

    this.loading = true;

    // Prepare user data (exclude confirmPassword)
    const userData: RegisterUserRequestDto = {
      username: this.f['username'].value,
      firstName: this.f['firstname'].value,
      lastName: this.f['lastname'].value,
      email: this.f['email'].value,
      password: this.f['password'].value,
      confirmPassword: this.f['confirmPassword'].value,
    };

    this.userService.register(userData).subscribe({

      next: (response) => {
        this.loading = false;
        this.success = true;

        if (response.user !== null) {
          this.router.navigate(['/login']);
        }

      },
      error: (error) => {
        this.loading = false;
        this.error =
          error.error?.message || 'Registration failed. Please try again.';
      },
    });


  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
}
