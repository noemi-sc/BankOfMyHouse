import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { UserService } from '../../services/users/users.service';
import { InvestmentService } from '../../dashboard/investments_dashboard/investment.service';
import { createInvestmentRequestDto } from '../models/create/createInvestmentRequestDto';

@Component({
  selector: 'app-create-investment',
  imports: [FormsModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './create-investment.component.html',
  styleUrls: ['./create-investment.component.css', '../../transactions/create/create-transaction-custom-style.scss']
})
export class CreateInvestmentComponent {

  // Inputs
  readonly initialValue = input<string>('');

  // Outputs
  readonly confirmed = output<string>();
  readonly cancelled = output<void>();

  // Local state
  protected readonly currentValue = signal('');
  protected readonly selectedSenderIban = signal<any>(null);
  protected readonly amount = signal<number | null>(null);
  protected readonly selectedCompany = signal<any>(null);
  protected readonly companies = signal<any[]>([]);

  private investmentService = inject(InvestmentService);
  private usersService = inject(UserService);



  // State signals from service
  currentUser = this.usersService.userDetails;
  companyName = this.investmentService.companyDetails;
  loading = this.investmentService.loading;
  error = this.investmentService.error;

  constructor() {
    // Initialize current value with initial value
    this.currentValue.set(this.initialValue());

    // Load companies from API
    this.investmentService.listCompanies().subscribe({
      next: (response) => {
        console.log('Companies API response:', response);
        // Handle server response structure
        if (response?.companies && Array.isArray(response.companies)) {
          this.companies.set(response.companies);
        } else if (response?.Companies && Array.isArray(response.Companies)) {
          this.companies.set(response.Companies);
        } else if (Array.isArray(response)) {
          this.companies.set(response);
        } else {
          this.companies.set([]);
        }
      }
    });
  }

  protected onConfirm(): void {
    // Validate required fields
    if (!this.selectedSenderIban() || !this.selectedCompany() || !this.amount() || this.amount()! <= 0) {
      return; // Don't proceed if validation fails
    }

    this.confirmed.emit(this.currentValue());

    var requestBody: createInvestmentRequestDto = new createInvestmentRequestDto();

    requestBody.companyId = this.selectedCompany().id;
    requestBody.investmentAmount = this.amount()!;
    requestBody.bankAccountId = this.selectedSenderIban().id;

    this.investmentService.createInvestment(requestBody).subscribe({
      next: (response) => {
        // Investment created successfully, emit success event
        this.cancelled.emit(); // Close the popup
      },
      error: (error) => {
        console.error('Investment creation failed:', error);
      }
    });
  }

  protected onOverlayClick(event: MouseEvent): void {
    // Close popup when clicking on overlay
    this.onCancel();
  }

  protected onCancel(): void {
    this.cancelled.emit();
  }

  protected isFormValid(): boolean {
    return !!(this.selectedSenderIban() && this.selectedCompany() && this.amount() && this.amount()! > 0);
  }
}
