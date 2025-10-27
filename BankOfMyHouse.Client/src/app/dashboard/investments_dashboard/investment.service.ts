import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { createInvestmentRequestDto } from '../../investments/models/create/createInvestmentRequestDto';
import { createInvestmentResponseDto } from '../../investments/models/create/createInvestmentResponseDto';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { listCompanyResponseDto } from '../../investments/models/listCompany/listCompanyResponseDto';

@Injectable({
  providedIn: 'root'
})
export class InvestmentService {
  private apiUrl =
    'http://localhost:57460/investments';

  private investmentDetailsSignal =
    signal<createInvestmentRequestDto | null>(null);
  private createInvestmentDetailsSignal =
    signal<createInvestmentResponseDto | null>(null);
  private loadingSignal =
    signal<boolean>(false);
  private errorSignal = signal<any>(null);

  public readonly companyDetails =
    this.createInvestmentDetailsSignal.asReadonly();
  public readonly loading =
    this.loadingSignal.asReadonly();
  public readonly error =
    this.errorSignal.asReadonly();

  private httpClient: HttpClient = inject(HttpClient);

  public createInvestment(body: createInvestmentRequestDto): Observable<createInvestmentResponseDto> {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    return this.httpClient
      .post<createInvestmentResponseDto>(`${this.apiUrl}`, body)
      .pipe(
        map((response) => {
          this.createInvestmentDetailsSignal.set(response);
          this.loadingSignal.set(false);
          return response;
        }),
        catchError((error) => {
          this.errorSignal.set(error);
          this.loadingSignal.set(false);
          return throwError(() => error);
        })
      );
  }

  listInvestment(): Observable<any> {
    return this.httpClient.get(`${this.apiUrl}`);
  }

  listCompanies(): Observable<any> {
    return this.httpClient.get(`${this.apiUrl}/companies`);
  }

  getHistoricalPrices(hours: number): Observable<any> {
    return this.httpClient.get(`${this.apiUrl}/historical-prices?hours=${hours}`);
  }
  /*   listCompanies(): Observable<listCompanyResponseDto> {
      return this.httpClient.get<listCompanyResponseDto>(`${this.apiUrl}/companies`);
    }
  */
}
