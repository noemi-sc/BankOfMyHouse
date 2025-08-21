import { Inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { GetUserDetailsResponseDto } from '../auth/models/getUserDetails/getUserDetailsResponseDto';
import { HttpClient } from '@angular/common/http';
import { map, catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private apiUrl =
    'http://localhost:57460/users';
  private userDetailsSignal =
    signal<GetUserDetailsResponseDto | null>(null);
  private loadingSignal =
    signal<boolean>(false);
  private errorSignal = signal<any>(null);

  public readonly userDetails =
    this.userDetailsSignal.asReadonly();
  public readonly loading =
    this.loadingSignal.asReadonly();
  public readonly error =
    this.errorSignal.asReadonly();

  constructor(private httpClient: HttpClient,
    @Inject(PLATFORM_ID) private platformId:
      Object) { }

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
}
