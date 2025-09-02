import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { GetBankAccountResponseDto } from '../models/list/GetBankAccountResponseDto';
import { BankAccountDto } from '../models/list/BankAccountDto';
import { BankAccountService } from '../bank-account.service';

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
  bankAccounts: BankAccountDto[] = [];

  constructor(private accountService: BankAccountService) {

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

