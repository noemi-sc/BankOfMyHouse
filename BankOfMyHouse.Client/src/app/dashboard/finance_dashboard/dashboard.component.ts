// components/dashboard/dashboard.component.ts
import { Component, computed, ChangeDetectionStrategy, inject, signal, OnInit, Signal } from '@angular/core';
import { CurrencyPipe, DecimalPipe, DatePipe } from '@angular/common';
import { UserService } from '../../services/users/users.service';
import { Router } from '@angular/router';
import { BankAccountService } from '../../account/bank-account.service';
import { CreateTransactionComponent } from '../../transactions/create/create-transaction.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { GetTransactionComponent } from "../../transactions/list/get-transaction.component";
import { MatGridListModule } from '@angular/material/grid-list';
import { TransactionService } from '../../transactions/transaction.service';
import { GetUserDetailsResponseDto } from '../../auth/models/getUserDetails/getUserDetailsResponseDto';
import { CreateBankAccountComponent } from '../../account/create/create-bank-account.component';

@Component({
  selector: 'app-dashboard',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  standalone: true,
  imports: [
    CurrencyPipe,
    DatePipe,
    CreateBankAccountComponent,
    CreateTransactionComponent,
    MatFormFieldModule,
    MatSelectModule,
    FormsModule,
    ReactiveFormsModule,
    GetTransactionComponent,
    MatGridListModule
  ],


})
export class DashboardComponent implements OnInit {

  private usersService = inject(UserService);
  private router = inject(Router);
  private bankAccountService = inject(BankAccountService);
  private transactionService = inject(TransactionService);
  protected readonly isBankAccountPopupOpen = signal(false);
  protected readonly isTransactionPopupOpen = signal(false);
  protected readonly inputValue = signal('');

  // State signals from service
  protected currentUser: Signal<GetUserDetailsResponseDto | null>;
  loading = this.usersService.loading;
  error = this.usersService.error;

  // Computed signal for total balance of all accounts
  totalBalance = computed(() => {
    if (!this.currentUser()?.bankAccounts) return 0;
    return this.currentUser()?.bankAccounts.reduce((sum, account) => sum + account.balance, 0);
  });

  totalAccounts = computed(() => {
    if (!this.currentUser()?.bankAccounts) return 0;
    return this.currentUser()?.bankAccounts.length;
  });

  constructor() {
    this.currentUser = this.usersService.userDetails;
  }

  ngOnInit(): void {
    this.usersService.getUserDetails();
  }

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