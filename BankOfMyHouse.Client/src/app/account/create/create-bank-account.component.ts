import { ChangeDetectionStrategy, Component, inject, Inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BankAccountService } from '../bank-account.service';
import { UsersService } from '../../users/users.service';
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
  // Inputs
  readonly initialValue = input<string>('');
  
  // Outputs
  readonly confirmed = output<string>();
  readonly cancelled = output<void>();


  // Local state
  protected readonly currentValue = signal('');

    private bankAccountService = inject(BankAccountService);
    private usersService = inject(UsersService);

  // State signals from service
  currentUser = this.usersService.userDetails;
  loading = this.bankAccountService.loading;
  error = this.bankAccountService.error;

constructor() {
    // Initialize current value with initial value
    this.currentValue.set(this.initialValue());
  }

  protected onConfirm(): void {
    this.confirmed.emit(this.currentValue());

    var requestBody: CreateBankAccountRequestDto = new CreateBankAccountRequestDto;

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
