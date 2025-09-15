import { Component, inject, OnInit, Signal, effect } from '@angular/core';
import { UserService } from '../../services/users/users.service';
import { TransactionService } from '../transaction.service';
import { GetUserDetailsResponseDto } from '../../auth/models/getUserDetails/getUserDetailsResponseDto';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { GetTransactionsResponseDto } from '../models/getTransactionsResponseDto';
import { GetTransactionsRequestDto } from '../models/getTransactionsRequestDto';

@Component({
  selector: 'app-get-transaction',
  // changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CurrencyPipe,
    DatePipe,
    MatPaginatorModule,
  ],
  templateUrl: './get-transaction.component.html',
  styleUrl: './get-transaction.component.css',
  standalone: true,
})
export class GetTransactionComponent implements OnInit {

  private usersService = inject(UserService);
  private transactionService = inject(TransactionService);

  // State signals from service
  protected currentUser: Signal<GetUserDetailsResponseDto | null>;
  protected currentTransactions: Signal<GetTransactionsResponseDto | null>;


  loading = this.usersService.loading;
  error = this.usersService.error;

  constructor() {
    this.currentUser = this.usersService.userDetails;
    this.currentTransactions = this.transactionService.getTransactions;

    // Effect to automatically fetch transactions when user data becomes available
    effect(() => {
      const user = this.currentUser();
      if (user?.bankAccounts) {
        user.bankAccounts.forEach(element => {
          const request = new GetTransactionsRequestDto();
          request.iban = element.iban;
          this.transactionService.getTransactionsDetails(request);
        });
      }
    });
  }

  ngOnInit() {
    this.usersService.getUserDetails();
  }
}
