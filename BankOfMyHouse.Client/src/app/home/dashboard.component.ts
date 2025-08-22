// components/dashboard/dashboard.component.ts
import { Component, computed, ChangeDetectionStrategy, inject, signal, OnInit, ViewEncapsulation } from '@angular/core';
import { CurrencyPipe, DecimalPipe, DatePipe } from '@angular/common';
import { UsersService } from '../users/users.service';
import { Router } from '@angular/router';
import { GetUserDetailsResponseDto } from '../auth/models/getUserDetails/getUserDetailsResponseDto';
import { BankAccountService } from '../bank-account.service';
import { CreateBankAccountComponent } from '../create-bank-account/create-bank-account.component';
import { CreateTransactionComponent } from '../transactions/create-transaction/create-transaction.component';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { GetTransactionComponent } from "../transactions/get-transaction/get-transaction.component";

interface Food {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'app-dashboard',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  standalone: true,
  imports: [
    CurrencyPipe,
    DecimalPipe,
    DatePipe,
    CreateBankAccountComponent,
    CreateTransactionComponent,
    MatFormFieldModule,
    MatSelectModule,
    FormsModule,
    ReactiveFormsModule
    /*     AccountCardComponent,
        InvestmentItemComponent,
        TransactionItemComponent,
        MarketOverviewComponent */
    ,
    GetTransactionComponent
],


})
export class DashboardComponent implements OnInit {

  private usersService = inject(UsersService);
  private router = inject(Router);
  private bankAccountService = inject(BankAccountService);
  protected readonly isBankAccountPopupOpen = signal(false);
  protected readonly isTransactionPopupOpen = signal(false);
  protected readonly inputValue = signal('');

  // State signals from service
  currentUser = this.usersService.userDetails;
  loading = this.usersService.loading;
  error = this.usersService.error;

  


  ngOnInit() {
    this.usersService.getUserDetails();
  }
  // dashboardService = inject(DashboardService);

//   portfolioChange = computed(() => this.dashboardService.portfolioChange());

  /*   recentTransactions = computed(() => 
      this.dashboardService.transactionsData()
        .slice()
        .sort((a, b) => b.date.getTime() - a.date.getTime())
        .slice(0, 5)
    ); */


  // Popup for new IBAN
  protected openBankAccountPopup(): void {
    this.isBankAccountPopupOpen.set(true);
  }

  protected onBankAccountPopupConfirmed(value: string): void {
    console.log('Bank account confirmed with value:', value);
    this.inputValue.set(value);
    this.isBankAccountPopupOpen.set(false);
  }

  protected onBankAccountPopupCancelled(): void {
    console.log('Bank account popup cancelled');
    this.isBankAccountPopupOpen.set(false);
  }

  // Popup for new payment

  protected openTransactionPopup(): void {
    this.isTransactionPopupOpen.set(true);
  }

  protected onTransactionPopupConfirmed(value: string): void {
    console.log('Transaction confirmed with value:', value);
    this.inputValue.set(value);
    this.isTransactionPopupOpen.set(false);
  }

  protected onTransactionPopupCancelled(): void {
    console.log('Transaction popup cancelled');
    this.isTransactionPopupOpen.set(false);
  }



}