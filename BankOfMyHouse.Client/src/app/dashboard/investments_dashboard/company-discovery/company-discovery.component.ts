import { Component, input, output, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from "@angular/material/card";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatDialog } from "@angular/material/dialog";
import { CreateInvestmentComponent } from "../../../investments/create-investment/create-investment.component";

@Component({
  selector: 'app-company-discovery',
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './company-discovery.component.html',
  styleUrl: './company-discovery.component.css'
})
export class CompanyDiscoveryComponent {
  private dialog = inject(MatDialog);

  // Inputs from parent
  readonly companies = input<any[]>([]);
  readonly searchQuery = input<string>('');

  // Outputs to parent
  readonly searchQueryChange = output<string>();
  readonly viewCompanyChart = output<any>();
  readonly investmentCreated = output<void>(); // Notify parent to reload data

  // Local search query for two-way binding
  protected localSearchQuery = signal<string>('');

  constructor() {
    // Sync with parent search query on input change
    this.localSearchQuery.set(this.searchQuery());
  }

  protected onSearchChange(value: string): void {
    this.localSearchQuery.set(value);
    this.searchQueryChange.emit(value);
  }

  protected onViewChart(company: any): void {
    this.viewCompanyChart.emit(company);
  }

  protected onCreateInvestment(companyId: number): void {
    const dialogRef = this.dialog.open(CreateInvestmentComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: { preSelectedCompanyId: companyId }
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result?.success) {
        // Notify parent to reload investments
        this.investmentCreated.emit();
      }
    });
  }
}
