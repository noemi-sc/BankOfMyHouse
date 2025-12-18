import { inject, Injectable, signal } from '@angular/core';
import { createInvestmentRequestDto } from '../../investments/models/create/createInvestmentRequestDto';
import { createInvestmentResponseDto } from '../../investments/models/create/createInvestmentResponseDto';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { GetHistoricalPricesResponseDto } from '../../investments/models/list_stock_history_price/getHistoricalPricesResponseDto';
import { listCompanyResponseDto } from '../../investments/models/listCompany/listCompanyResponseDto';
import { GetInvestmentsResponseDto } from '../../investments/models/list_investments/getInvestmentsResponseDto';

@Injectable({
  providedIn: 'root'
})
export class InvestmentService {
  private apiUrl =
    'http://localhost:57460/investments';

  private createInvestmentDetailsSignal =
    signal<createInvestmentResponseDto | null>(null);
  private loadingSignal =
    signal<boolean>(false);
  private errorSignal = signal<HttpErrorResponse | Error | null>(null);

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

  listInvestment(): Observable<GetInvestmentsResponseDto> {
    return this.httpClient.get<GetInvestmentsResponseDto>(`${this.apiUrl}`);
  }

  listCompanies(): Observable<listCompanyResponseDto> {
    return this.httpClient.get<listCompanyResponseDto>(`${this.apiUrl}/companies`);
  }

  getHistoricalPrices(hours?: number, companyId?: number, startDate?: Date, endDate?: Date): Observable<GetHistoricalPricesResponseDto> {
    let url = `${this.apiUrl}/historical-prices?`;

    // Priority: use startDate if provided, otherwise use hours
    if (startDate) {
      url += `startDate=${startDate.toISOString()}`;
      if (endDate) {
        url += `&endDate=${endDate.toISOString()}`;
      }
    } else {
      url += `hours=${hours || 12}`;
    }

    if (companyId) {
      url += `&companyId=${companyId}`;
    }

    return this.httpClient.get<GetHistoricalPricesResponseDto>(url);
  }
}
