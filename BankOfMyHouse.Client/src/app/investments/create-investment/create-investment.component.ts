import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
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
  private dialogRef = inject(MatDialogRef<CreateInvestmentComponent>);
  private data = inject(MAT_DIALOG_DATA, { optional: true });

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
    // Load companies from API
    this.investmentService.listCompanies().subscribe({
      next: (response) => {
        console.log('Companies API response:', response);
        // Handle server response structure
        if (response?.companies && Array.isArray(response.companies)) {
          this.companies.set(response.companies);
        } else if (response?.companies && Array.isArray(response.companies)) {
          this.companies.set(response.companies);
        } else if (Array.isArray(response)) {
          this.companies.set(response);
        } else {
          this.companies.set([]);
        }

        // Pre-select company if provided via dialog data
        const preSelectedId = this.data?.preSelectedCompanyId;
        if (preSelectedId) {
          const companyToSelect = response.companies?.find((c: any) => c.id === preSelectedId);
          if (companyToSelect) {
            this.selectedCompany.set(companyToSelect);
            console.log('Pre-selected company:', companyToSelect);
          }
        }
      }
    });
  }

  protected onConfirm(): void {
    // Validate required fields
    if (!this.selectedSenderIban() || !this.selectedCompany() || !this.amount() || this.amount()! <= 0) {
      return; // Don't proceed if validation fails
    }

    var requestBody: createInvestmentRequestDto = new createInvestmentRequestDto();

    requestBody.companyId = this.selectedCompany().id;
    requestBody.investmentAmount = this.amount()!;
    requestBody.bankAccountId = this.selectedSenderIban().id;

    this.investmentService.createInvestment(requestBody).subscribe({
      next: (response) => {
        // Investment created successfully, close dialog with success
        this.dialogRef.close({ success: true });
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
    this.dialogRef.close({ success: false });
  }

  protected isFormValid(): boolean {
    return !!(this.selectedSenderIban() && this.selectedCompany() && this.amount() && this.amount()! > 0);
  }
}
