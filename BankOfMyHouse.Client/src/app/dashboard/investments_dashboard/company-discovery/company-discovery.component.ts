import { Component, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from "@angular/material/card";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";

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
  // Inputs from parent
  readonly companies = input<any[]>([]);
  readonly searchQuery = input<string>('');

  // Outputs to parent
  readonly searchQueryChange = output<string>();
  readonly viewCompanyChart = output<any>();
  readonly createInvestment = output<void>();

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

  protected onCreateInvestment(): void {
    this.createInvestment.emit();
  }
}
