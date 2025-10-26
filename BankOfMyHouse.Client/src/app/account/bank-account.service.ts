import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, catchError, throwError, Observable } from 'rxjs';
import { CreateBankAccountResponseDto } from './models/create/CreateBankAccountResponseDto';
import { CreateBankAccountRequestDto } from './models/create/CreateBankAccountRequestDto';
import { BankAccountDto } from './models/list/BankAccountDto';
import { GetBankAccountResponseDto } from './models/list/GetBankAccountResponseDto';
@Injectable({
  providedIn: 'root'
})
export class BankAccountService {

  private apiUrl =
    'http://localhost:57460/bankAccounts';
  private bankAccountSignal =
    signal<CreateBankAccountResponseDto | null>(null);
  private bankAccountsListSignal =
    signal<BankAccountDto[]>([]);
  private loadingSignal =
    signal<boolean>(false);
  private errorSignal = signal<any>(null);
  private bankAccountCreatedSignal =
    signal<boolean>(false);

  public readonly loading =
    this.loadingSignal.asReadonly();
  public readonly error =
    this.errorSignal.asReadonly();
  public readonly bankAccountsList =
    this.bankAccountsListSignal.asReadonly();
  public readonly accountCreated =
    this.bankAccountCreatedSignal.asReadonly();

  private httpClient = inject(HttpClient);

  public loadBankAccounts(): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.httpClient
      .get<GetBankAccountResponseDto>(`${this.apiUrl}`)
      .pipe(
        map((response) => response.BankAccounts),
        catchError((error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
          return throwError(() => error);
        })
      )
      .subscribe({
        next: (accounts) => {
          this.bankAccountsListSignal.set(accounts);
          this.loadingSignal.set(false);
        },
        error: (error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
        }
      });
  }

  public createBankAccount(body: CreateBankAccountRequestDto): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);
    this.bankAccountCreatedSignal.set(false);

    this.httpClient
      .post<CreateBankAccountResponseDto>(`${this.apiUrl}`, body)
      .pipe(
        map((response) => {
          return response;
        }),
        catchError((error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
          this.bankAccountCreatedSignal.set(false);
          return throwError(() => error);
        })
      )
      .subscribe({
        next: (response) => {
          this.bankAccountSignal.set(response);
          // Keep loading state true and refresh the list
          this.refreshBankAccountsAfterCreate();
        },
        error: (error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
          this.bankAccountCreatedSignal.set(false);
        }
      });
  }

  private refreshBankAccountsAfterCreate(): void {
    this.httpClient
      .get<GetBankAccountResponseDto>(`${this.apiUrl}`)
      .pipe(
        map((response) => response.BankAccounts),
        catchError((error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
          this.bankAccountCreatedSignal.set(false);
          return throwError(() => error);
        })
      )
      .subscribe({
        next: (accounts) => {
          this.bankAccountsListSignal.set(accounts);
          this.loadingSignal.set(false);
          this.bankAccountCreatedSignal.set(true);
        },
        error: (error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
          this.bankAccountCreatedSignal.set(false);
        }
      });
  }

  listBankAccounts(): Observable<any> {
    return this.httpClient.get(`${this.apiUrl}`);
  }
}


