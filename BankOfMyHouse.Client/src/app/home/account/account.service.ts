import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private apiUrl = 'http://localhost:57460/bankAccounts';

  constructor(
    private httpClient: HttpClient
  ) {
    this.httpClient = httpClient;
  }

  listAccounts(): Observable<any> {
    return this.httpClient.get(`${this.apiUrl}`);
  }
}
