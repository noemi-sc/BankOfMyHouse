import { Injectable, inject, PLATFORM_ID, signal } from '@angular/core';
import { GetUserDetailsResponseDto } from '../../auth/models/getUserDetails/getUserDetailsResponseDto';
import { HttpClient } from '@angular/common/http';
import { map, catchError, throwError, Observable } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { UserLoginRequestDto, UserLoginResponseDto, RegisterUserRequestDto, RegisterUserResponseDto } from '../../auth/models/auth-response';
import { AUTH_CONSTANTS } from '../../shared/constants/auth.constants';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl: string ='http://localhost:57460/users';
  private userDetailsSignal = signal<GetUserDetailsResponseDto | null>(null);
  private loadingSignal = signal<boolean>(false);
  private errorSignal = signal<any>(null);
  private currentUserSignal = signal<any>(null);
  private isAuthenticatedSignal = signal<boolean>(false);

  public readonly userDetails =
    this.userDetailsSignal.asReadonly();
  public readonly loading =
    this.loadingSignal.asReadonly();
  public readonly error =
    this.errorSignal.asReadonly();
  public readonly currentUser =
    this.currentUserSignal.asReadonly();
  public readonly isAuthenticated =
    this.isAuthenticatedSignal.asReadonly();

  private refreshTimer: any;

  private httpClient = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);

  constructor() {
    this.checkInitialAuthState();
  }

  public getUserDetails(): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.httpClient
      .get<GetUserDetailsResponseDto>(`${this.apiUrl}/details`)
      .pipe(
        map((response) => {
          return response;
        }),
        catchError((error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
          return throwError(() => error);
        })
      )
      .subscribe({
        next: (response) => {

          this.userDetailsSignal.set(response);
          this.loadingSignal.set(false);
        },
        error: (error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
        }
      });
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
          this.isAuthenticatedSignal.set(true);
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

  login(userLoginRequest: UserLoginRequestDto): Observable<UserLoginResponseDto> {
    this.loadingSignal.set(true);

    return this.httpClient
      .post<UserLoginResponseDto>(`${this.apiUrl}/auth/login`, userLoginRequest)
      .pipe(
        map((response) => {
          this.storeAuthData(response);
          this.loadingSignal.set(false);
          return response;
        }),
        catchError((error) => {
          this.loadingSignal.set(false);
          this.errorSignal.set(error);
          return throwError(() => error);
        })
      );
  }

  register(registerUserRequestDto: RegisterUserRequestDto): Observable<RegisterUserResponseDto> {
    this.loadingSignal.set(true);

    return this.httpClient
      .post<RegisterUserResponseDto>(`${this.apiUrl}/auth/register`, registerUserRequestDto)
      .pipe(
        map((response) => {
          this.loadingSignal.set(false);
          return response;
        }),
        catchError((error) => {
          this.loadingSignal.set(false);
          this.errorSignal.set(error);
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
    this.currentUserSignal.set(response.user);
    this.isAuthenticatedSignal.set(true);

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

  public clearAuthData(): void {
    // Clear timer
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
    }

    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(AUTH_CONSTANTS.TOKEN_KEY);
      localStorage.removeItem(AUTH_CONSTANTS.REFRESH_TOKEN_KEY);
      localStorage.removeItem(AUTH_CONSTANTS.EXPIRES_AT_KEY);
    }

    this.currentUserSignal.set(null);
    this.isAuthenticatedSignal.set(false);
  }
}
