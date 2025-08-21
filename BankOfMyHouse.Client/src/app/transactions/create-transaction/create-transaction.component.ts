import { ChangeDetectionStrategy, Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BankAccountService } from '../../bank-account.service';
import { CreateTransactionRequestDto, IbanCodeDto } from '../models/createTransactionRequestDto';
import { TransactionService } from '../transaction.service';
import { UsersService } from '../../users/users.service';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';

@Component({
  selector: 'app-create-transaction',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './create-transaction.component.html',
  styleUrl: './create-transaction.component.css'
})
export class CreateTransactionComponent {
  // Inputs
  readonly initialValue = input<string>('');

  // Outputs
  readonly confirmed = output<string>();
  readonly cancelled = output<void>();


  // Local state
  protected readonly currentValue = signal('');
  protected readonly selectedSenderIban = signal<any>(null);
  protected readonly receiverIban = signal('');
  protected readonly amount = signal(0);

  private transactionService = inject(TransactionService);
  private usersService = inject(UsersService);

  // State signals from service
  currentUser = this.usersService.userDetails;
  loading = this.transactionService.loading;
  error = this.transactionService.error;

  constructor() {
    // Initialize current value with initial value
    this.currentValue.set(this.initialValue());
  }

  protected onConfirm(): void {
    this.confirmed.emit(this.currentValue());

    var requestBody: CreateTransactionRequestDto = new CreateTransactionRequestDto();

    // Set sender IBAN from selected bank account
    if (this.selectedSenderIban()) {
      requestBody.senderIban = new IbanCodeDto(this.selectedSenderIban().iban);
    }
    
    // Set receiver IBAN from form input
    requestBody.receiverIban = new IbanCodeDto(this.receiverIban());
    
    // Set amount
    requestBody.amount = this.amount();
    
    // Set description (causale)
    requestBody.description = this.currentValue();

    this.transactionService.createTransaction(requestBody);
  }

  protected onCancel(): void {
    this.cancelled.emit();
  }

  protected onOverlayClick(event: MouseEvent): void {
    // Close popup when clicking on overlay
    this.onCancel();
  }
}

