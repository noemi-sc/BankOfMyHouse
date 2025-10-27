// components/dashboard/dashboard.component.ts
import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { UserService } from '../../services/users/users.service';
import { CreateTransactionComponent } from '../../transactions/create/create-transaction.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { GetTransactionComponent } from "../../transactions/list/get-transaction.component";
import { CreateBankAccountComponent } from '../../bankAccounts/create/create-bank-account.component';
import { ListBankAccountsComponent } from '../../bankAccounts/list-bank-accounts/list-bank-accounts.component';

@Component({
  selector: 'app-dashboard',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  standalone: true,
  imports: [
    CreateBankAccountComponent,
    CreateTransactionComponent,
    ListBankAccountsComponent,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    GetTransactionComponent
  ],
})
export class DashboardComponent implements OnInit {

  private usersService = inject(UserService);

  protected readonly isBankAccountPopupOpen = signal(false);
  protected readonly isTransactionPopupOpen = signal(false);

  // State signals from service
  protected currentUser = this.usersService.userDetails;
  protected loading = this.usersService.loading;
  protected error = this.usersService.error;

  ngOnInit(): void {
    this.usersService.getUserDetails();
  }

  // Popup for new IBAN
  protected openBankAccountPopup(): void {
    this.isBankAccountPopupOpen.set(true);
  }

  protected onBankAccountPopupConfirmed(): void {
    this.isBankAccountPopupOpen.set(false);
    // Refresh user details to show the newly created bank account
    this.usersService.getUserDetails();
  }

  protected onBankAccountPopupCancelled(): void {
    this.isBankAccountPopupOpen.set(false);
  }

  // Popup for new payment
  protected openTransactionPopup(): void {
    this.isTransactionPopupOpen.set(true);
  }

  protected onTransactionPopupConfirmed(): void {
    this.isTransactionPopupOpen.set(false);
    // Refresh user details to show updated balances after transaction
    this.usersService.getUserDetails();
  }

  protected onTransactionPopupCancelled(): void {
    this.isTransactionPopupOpen.set(false);
  }
}
