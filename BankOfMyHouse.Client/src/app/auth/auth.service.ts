import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, finalize, map, Observable, throwError } from 'rxjs';
import {
  UserLoginRequestDto,
  UserLoginResponseDto,
  RegisterUserRequestDto,
  RegisterUserResponseDto,
} from './models/auth-response';
import { UserDto } from './models/user';
import { isPlatformBrowser } from '@angular/common';
import { AUTH_CONSTANTS } from '../shared/constants/auth.constants';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // API Configuration
  private apiUrl = 'http://localhost:57460/users';

  // State Management
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  private refreshTimer: any;

  constructor(
    private httpClient: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.checkInitialAuthState();
  }

  public get currentUserValue(): UserDto {
    return this.currentUserSubject.value;
  }

  private checkInitialAuthState(): void {
    if (isPlatformBrowser(this.platformId)) {
      const token = localStorage.getItem(AUTH_CONSTANTS.TOKEN_KEY);
      const expiresAt = localStorage.getItem(AUTH_CONSTANTS.EXPIRES_AT_KEY);

      if (token && expiresAt) {
        const expirationDate = new Date(expiresAt);
        const now = new Date();

        if (expirationDate > now) {
          // Token is still valid
          this.isAuthenticatedSubject.next(true);
          this.scheduleTokenRefresh(expirationDate);

          // TODO: Optionally decode token to get user info or fetch user profile
        } else {
          // Token is expired, try to refresh
          this.attemptTokenRefresh();
        }
      } else {
        this.clearAuthData();
      }
    }
  }

  login(userLoginRequest: UserLoginRequestDto): Observable<any> {
    this.loadingSubject.next(true);

    return this.httpClient
      .post<UserLoginResponseDto>(`${this.apiUrl}/auth/login`, userLoginRequest)
      .pipe(
        map((response) => {
          this.storeAuthData(response);
          this.loadingSubject.next(false);
          return response;
        }),
        catchError((error) => {
          this.loadingSubject.next(false);
          return throwError(() => error);
        })
      );
  }

  register(registerUserRequestDto: RegisterUserRequestDto): Observable<any> {
    this.loadingSubject.next(true);

    return this.httpClient
      .post<RegisterUserResponseDto>(`${this.apiUrl}/auth/register`, registerUserRequestDto)
      .pipe(
        map((response) => {
          this.loadingSubject.next(false);
          return response;
        }),
        catchError((error) => {
          this.loadingSubject.next(false);
          return throwError(() => error);
        })
      );
  }

  logout(): Observable<any> {
    this.loadingSubject.next(true);

    return this.httpClient
      .post(`${this.apiUrl}/auth/logout`, {})
      .pipe(
        finalize(() => {
          this.clearAuthData();
          this.loadingSubject.next(false);
        }),
        catchError((error) => {
          return throwError(() => error);
        })
      );
  }

  // New: Token refresh method
  refreshToken(): Observable<UserLoginResponseDto> {
    if (!isPlatformBrowser(this.platformId)) {
      return throwError(() => new Error('Not in browser environment'));
    }

    const refreshToken = localStorage.getItem(AUTH_CONSTANTS.REFRESH_TOKEN_KEY);
    if (!refreshToken) {
      this.clearAuthData();
      return throwError(() => new Error('No refresh token available'));
    }

    return this.httpClient
      .post<UserLoginResponseDto>(`${this.apiUrl}/auth/refresh`, {
        refreshToken: refreshToken
      })
      .pipe(
        map((response) => {
          this.storeAuthData(response);
          return response;
        }),
        catchError((error) => {
          // Refresh failed, logout user
          this.clearAuthData();
          return throwError(() => error);
        })
      );
  }

  // New: Store all auth data
  private storeAuthData(response: UserLoginResponseDto): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(AUTH_CONSTANTS.TOKEN_KEY, response.accessToken);
      localStorage.setItem(AUTH_CONSTANTS.REFRESH_TOKEN_KEY, response.refreshToken);
      localStorage.setItem(AUTH_CONSTANTS.EXPIRES_AT_KEY, response.expiresAt);

      console.log('🔍 Auth data stored:');
      console.log('Token:', response.accessToken);
      console.log('Expires at:', response.expiresAt);
    }

    // Update state
    this.currentUserSubject.next(response.user);
    this.isAuthenticatedSubject.next(true);

    // Schedule token refresh
    this.scheduleTokenRefresh(new Date(response.expiresAt));
  }

  // New: Schedule automatic token refresh
  private scheduleTokenRefresh(expirationDate: Date): void {
    // Clear existing timer
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
    }

    const now = new Date().getTime();
    const expiration = expirationDate.getTime();

    // Refresh 5 minutes before expiration
    const refreshTime = expiration - now - (5 * 60 * 1000);

    if (refreshTime > 0) {
      console.log(`🔍 Token refresh scheduled in ${refreshTime / 1000} seconds`);

      this.refreshTimer = setTimeout(() => {
        this.attemptTokenRefresh();
      }, refreshTime);
    } else {
      // Token expires very soon, refresh immediately
      this.attemptTokenRefresh();
    }
  }

  // New: Attempt to refresh token
  private attemptTokenRefresh(): void {
    this.refreshToken().subscribe({
      next: (response) => {
        console.log('🔍 Token refreshed successfully');
      },
      error: (error) => {
        console.log('🔍 Token refresh failed, logging out');
        this.clearAuthData();
      }
    });
  }

  private clearAuthData(): void {
    // Clear timer
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
    }

    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(AUTH_CONSTANTS.TOKEN_KEY);
      localStorage.removeItem(AUTH_CONSTANTS.REFRESH_TOKEN_KEY);
      localStorage.removeItem(AUTH_CONSTANTS.EXPIRES_AT_KEY);
    }

    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  getToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(AUTH_CONSTANTS.TOKEN_KEY);
    }
    return null;
  }

  isLoggedIn(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  // New: Check if token is expired
  isTokenExpired(): boolean {
    if (!isPlatformBrowser(this.platformId)) {
      return true;
    }

    const expiresAt = localStorage.getItem(AUTH_CONSTANTS.EXPIRES_AT_KEY);
    if (!expiresAt) {
      return true;
    }

    const expirationDate = new Date(expiresAt);
    const now = new Date();

    return expirationDate <= now;
  }
}