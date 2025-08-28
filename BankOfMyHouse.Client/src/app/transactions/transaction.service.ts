import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateTransactionRequestDto } from './models/createTransactionRequestDto';
import { CreateTransactionResponseDto } from './models/createTransactionResponseDto';
import { map, catchError, throwError } from 'rxjs';
import { GetTransactionsResponseDto } from './models/getTransactionsResponseDto';
import { GetTransactionsRequestDto } from './models/getTransactionsRequestDto';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private apiUrl =
    'http://localhost:57460/transactions';

  private transactionDetailsSignal =
    signal<CreateTransactionResponseDto | null>(null);
  private getTransactionDetailsSignal =
    signal<GetTransactionsResponseDto | null>(null);
  private loadingSignal =
    signal<boolean>(false);
  private errorSignal = signal<any>(null);
  private httpClient = inject(HttpClient);

  public readonly getTransaction =
    this.getTransactionDetailsSignal.asReadonly();
  public readonly loading =
    this.loadingSignal.asReadonly();
  public readonly error =
    this.errorSignal.asReadonly();

  public createTransaction(body: CreateTransactionRequestDto): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.httpClient
      .post<CreateTransactionResponseDto>(`${this.apiUrl}`, body)
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

          this.transactionDetailsSignal.set(response);
          this.loadingSignal.set(false);
        },
        error: (error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
        }
      });
  }

  public getTransactionsDetails(getTransactionsRequestDto: GetTransactionsRequestDto): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.httpClient
      .get<GetTransactionsResponseDto>(`${this.apiUrl}?iban=${getTransactionsRequestDto.iban}`)//&startDate=${getTransactionsRequestDto.startDate}`)
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

          this.getTransactionDetailsSignal.set(response);
          this.loadingSignal.set(false);
        },
        error: (error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
        }
      });
  }
}