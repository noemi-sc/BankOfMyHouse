import { ChangeDetectionStrategy, Component, inject, input, output, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateTransactionRequestDto, IbanCodeDto } from '../models/createTransactionRequestDto';
import { TransactionService } from '../transaction.service';
import { UserService } from '../../services/users/users.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-create-transaction',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './create-transaction.component.html',
  styleUrls: ['./create-transaction.component.css', './create-transaction-custom-style.scss']
})
export class CreateTransactionComponent {
  // Outputs
  readonly confirmed = output<void>();
  readonly cancelled = output<void>();

  // Local state
  protected readonly currentValue = signal('');
  protected readonly selectedSenderIban = signal<any>(null);
  protected readonly receiverIban = signal('');
  protected readonly amount = signal<number | null>(null);
  protected readonly currencies = [
    { name: "US Dollar", code: "USD", symbol: "$" },
    { name: "Euro", code: "EUR", symbol: "€" },
    { name: "British Pound", code: "GBP", symbol: "£" }
  ];
  protected readonly selectedCurrency = signal<any>(this.currencies[1].code);

  private transactionService = inject(TransactionService);
  private usersService = inject(UserService);

  // State signals from service
  protected currentUser = this.usersService.userDetails;
  protected loading = this.transactionService.loading;
  protected error = this.transactionService.error;

  // Computed signal for error message
  protected readonly errorMessage = computed(() => {
    const err = this.error();
    if (!err) return null;

    // Extract error message from the API response
    if (err?.error?.detail) {
      return err.error.detail;
    } else if (err?.error?.title) {
      return err.error.title;
    } else if (err?.message) {
      return err.message;
    }
    return 'Si è verificato un errore durante la transazione';
  });

  // Computed signal for form validation
  protected readonly isFormValid = computed(() => {
    return !!(
      this.selectedSenderIban() &&
      this.receiverIban() &&
      this.receiverIban().trim() !== '' &&
      this.amount() !== null &&
      this.amount() !== undefined &&
      this.amount()! > 0 &&
      this.selectedCurrency()
    );
  });

  protected onConfirm(): void {
    const requestBody: CreateTransactionRequestDto = new CreateTransactionRequestDto();

    // Set sender IBAN from selected bank account
    if (this.selectedSenderIban()) {
      requestBody.senderIban = new IbanCodeDto(this.selectedSenderIban().iban);
    }

    // Set receiver IBAN from form input
    requestBody.receiverIban = new IbanCodeDto(this.receiverIban());

    // Set amount
    requestBody.amount = this.amount() ?? 0;

    // Set currency
    requestBody.currencyCode = this.selectedCurrency().code;

    // Set description (causale)
    requestBody.description = this.currentValue();

    this.transactionService.createTransaction(requestBody);
    this.confirmed.emit();
  }

  protected onCancel(): void {
    this.cancelled.emit();
  }

  protected onOverlayClick(event: MouseEvent): void {
    // Close popup when clicking on overlay
    this.onCancel();
  }
}

