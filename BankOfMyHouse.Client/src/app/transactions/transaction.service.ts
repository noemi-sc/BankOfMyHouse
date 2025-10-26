import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateTransactionRequestDto } from './models/createTransactionRequestDto';
import { CreateTransactionResponseDto } from './models/createTransactionResponseDto';
import { map, catchError, throwError, Observable } from 'rxjs';
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
  private refreshTriggerSignal = signal<number>(0);
  private httpClient = inject(HttpClient);

  public readonly getTransactions =
    this.getTransactionDetailsSignal.asReadonly();
  public readonly loading =
    this.loadingSignal.asReadonly();
  public readonly error =
    this.errorSignal.asReadonly();
  public readonly refreshTrigger =
    this.refreshTriggerSignal.asReadonly();

  public createTransaction(body: CreateTransactionRequestDto): Observable<CreateTransactionResponseDto> {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    return this.httpClient
      .post<CreateTransactionResponseDto>(`${this.apiUrl}`, body)
      .pipe(
        map((response) => {
          this.transactionDetailsSignal.set(response);
          this.loadingSignal.set(false);
          // Delay refresh to allow backend to process the transaction
          setTimeout(() => {
            this.triggerRefresh();
          }, 500);
          return response;
        }),
        catchError((error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
          return throwError(() => error);
        })
      );
  }

  public getTransactionsDetails(getTransactionsRequestDto: GetTransactionsRequestDto): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    // Build query parameters
    let queryParams = `iban=${getTransactionsRequestDto.iban}`;

    // Add optional date parameters if provided
    if (getTransactionsRequestDto.startDate) {
      queryParams += `&startDate=${getTransactionsRequestDto.startDate.toISOString()}`;
    }
    if (getTransactionsRequestDto.endDate) {
      queryParams += `&endDate=${getTransactionsRequestDto.endDate.toISOString()}`;
    }

    this.httpClient
      .get<GetTransactionsResponseDto>(`${this.apiUrl}?${queryParams}`)
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
          // Accumulate transactions without duplicates
          const currentTransactions = this.getTransactionDetailsSignal();
          const newTransactionsData = new GetTransactionsResponseDto();
          
          if (currentTransactions && currentTransactions.transactions) {
            // Create a Map to avoid duplicates by transaction ID
            const transactionMap = new Map();
            
            // Add existing transactions
            currentTransactions.transactions.forEach(tx => {
              transactionMap.set(tx.id, tx);
            });
            
            // Add new transactions (will overwrite duplicates)
            response.transactions.forEach(tx => {
              transactionMap.set(tx.id, tx);
            });
            
            // Convert back to array
            newTransactionsData.transactions = Array.from(transactionMap.values());
          } else {
            newTransactionsData.transactions = response.transactions;
          }
          
          this.getTransactionDetailsSignal.set(newTransactionsData);
          this.loadingSignal.set(false);
        },
        error: (error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
        }
      });
  }

  public triggerRefresh(): void {
    this.refreshTriggerSignal.set(this.refreshTriggerSignal() + 1);
  }

  public clearTransactions(): void {
    this.getTransactionDetailsSignal.set(null);
  }
}