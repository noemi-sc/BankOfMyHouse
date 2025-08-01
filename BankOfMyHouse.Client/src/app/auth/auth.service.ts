import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, map, Observable } from 'rxjs';
import {
  UserLoginRequest,
  UserLoginResponse,
  RegisterUserRequestDto,
  RegisterUserResponseDto,
} from './models/auth-response';
import { UserDto } from './models/user';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // API Configuration
  private apiUrl = 'http://localhost:57460/users';
  private readonly TOKEN_KEY = '';

  // State Management
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  constructor(
    private httpClient: HttpClient,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object // Add this
  ) {
    this.httpClient = httpClient;
    this.router = router;
    // Initialize auth state on service creation
    this.checkInitialAuthState();
  }

  // Helper getter
  public get currentUserValue(): UserDto {
    return this.currentUserSubject.value;
  }

  private checkInitialAuthState(): void {
    // Only access localStorage in browser environment
    if (isPlatformBrowser(this.platformId)) {
      const token = localStorage.getItem(this.TOKEN_KEY);
      if (token) {
        this.isAuthenticatedSubject.next(true);
        // Optionally decode token to get user info
      } else {
        this.isAuthenticatedSubject.next(false);
        this.currentUserSubject.next(null);
      }
    }
  }

  login(userLoginRequest: UserLoginRequest): Observable<any> {
    this.loadingSubject.next(true);

    return this.httpClient
      .post<UserLoginResponse>(`${this.apiUrl}/auth/login`, {
        userLoginRequest: userLoginRequest,
      })
      .pipe(
        map((response) => {
          if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem(this.TOKEN_KEY, response.accessToken);
          }

          // Update state
          this.currentUserSubject.next(response.user);
          this.isAuthenticatedSubject.next(true);
          this.loadingSubject.next(false);

          return response;
        }),
        catchError((error) => {
          this.loadingSubject.next(false);
          return error(error);
        })
      );
  }

  register(registerUserRequestDto: RegisterUserRequestDto): Observable<any> {
    this.loadingSubject.next(true);

    return this.httpClient
      .post<RegisterUserResponseDto>(
        `${this.apiUrl}/auth/register`,
        registerUserRequestDto
      )
      .pipe(
        map((response) => {
          this.loadingSubject.next(false);
          return response;
        }),
        catchError((error) => {
          this.loadingSubject.next(false);
          return error(error);
        })
      );
  }

  logout(): void {
    // Remove token
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.TOKEN_KEY);
    }

    // Reset state
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);

    // Navigate to login
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(this.TOKEN_KEY);
    }
    return null;
  }

  isLoggedIn(): boolean {
    return this.isAuthenticatedSubject.value;
  }
}
