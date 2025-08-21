import { Inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, catchError, throwError } from 'rxjs';
import { CreateBankAccountResponseDto } from './models/bankAccounts/create/CreateBankAccountResponseDto';
import { CreateBankAccountRequestDto } from './models/bankAccounts/create/CreateBankAccountRequestDto';
@Injectable({
  providedIn: 'root'
})
export class BankAccountService {

  private apiUrl =
    'http://localhost:57460/bankAccounts';
  private bankAccountSignal =
    signal<CreateBankAccountResponseDto | null>(null);
  private loadingSignal =
    signal<boolean>(false);
  private errorSignal = signal<any>(null);

  public readonly loading =
    this.loadingSignal.asReadonly();
  public readonly error =
    this.errorSignal.asReadonly();

  constructor(private httpClient: HttpClient,
    @Inject(PLATFORM_ID) private platformId:
      Object) { }

  public createBankAccount(body: CreateBankAccountRequestDto): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.httpClient
      .post<CreateBankAccountResponseDto>(`${this.apiUrl}`, body)
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

          this.bankAccountSignal.set(response);
          this.loadingSignal.set(false);
        },
        error: (error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
        }
      });
  }
}


