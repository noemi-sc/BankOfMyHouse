import { ChangeDetectionStrategy, Component, inject, output, signal, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BankAccountService } from '../bank-account.service';
import { UserService } from '../../services/users/users.service';
import { CreateBankAccountRequestDto } from '../models/create/CreateBankAccountRequestDto';

@Component({
  selector: 'app-create-bank-account',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule],
  templateUrl: './create-bank-account.component.html',
  styleUrl: './create-bank-account.component.css'
})
export class CreateBankAccountComponent {
  // Outputs
  readonly confirmed = output<void>();
  readonly cancelled = output<void>();

  // Local state
  protected readonly currentValue = signal('');

  private bankAccountService = inject(BankAccountService);
  private usersService = inject(UserService);

  // State signals from service
  protected currentUser = this.usersService.userDetails;
  protected loading = this.bankAccountService.loading;
  protected error = this.bankAccountService.error;

  constructor() {
    // Watch for account creation completion
    effect(() => {
      if (this.bankAccountService.accountCreated()) {
        this.confirmed.emit();
      }
    });
  }

  protected onConfirm(): void {
    const requestBody: CreateBankAccountRequestDto = new CreateBankAccountRequestDto();
    requestBody.description = this.currentValue();

    this.bankAccountService.createBankAccount(requestBody);
  }

  protected onCancel(): void {
    this.cancelled.emit();
  }

  protected onOverlayClick(event: MouseEvent): void {
    // Close popup when clicking on overlay
    this.onCancel();
  }
}
