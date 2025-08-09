import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { AccountService } from '../account.service';
import { BankAccount, GetBankAccountResponseDto } from '../models/accounts';

/**
 * @title List with single selection using Reactive forms
 */
@Component({
  selector: 'app-account-list',
  standalone: true,
  imports: [MatListModule, FormsModule, ReactiveFormsModule],
  templateUrl: './list.component.html',
  styleUrl: './list.component.css'
})
export class ListAccountComponent {
  form: FormGroup<any>;
  bankAccountsControl: FormControl<any> = new FormControl<any>(null);
  bankAccounts: BankAccount[] = [];

  constructor(private accountService: AccountService) {

    this.form = new FormGroup({
      accountsList: this.bankAccountsControl,
    });

    this.accountService.listAccounts().subscribe({
      next: (data: GetBankAccountResponseDto) => {
        console.log('Accounts:', data);
        this.bankAccounts = data.BankAccounts;
      },
      error: (error) => {
        console.error('Error fetching accounts:', error);
      }
    });

  }
}

