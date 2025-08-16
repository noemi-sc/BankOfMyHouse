// components/dashboard/dashboard.component.ts
import { Component, computed, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { UserDto } from '../auth/models/user';
/* import { DashboardService } from '../../services/dashboard.service';
import { AccountCardComponent } from '../account-card/account-card.component';
import { InvestmentItemComponent } from '../investment-item/investment-item.component';
import { TransactionItemComponent } from '../transaction-item/transaction-item.component';
import { MarketOverviewComponent } from '../market-overview/market-overview.component'; */

@Component({
  selector: 'app-dashboard',
  changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  standalone: true,
  imports: [
    CurrencyPipe,
    DecimalPipe,
/*     AccountCardComponent,
    InvestmentItemComponent,
    TransactionItemComponent,
    MarketOverviewComponent */
  ],


})
export class DashboardComponent {
  // State signals
  currentUser = signal<UserDto | null>({
    email: 'John Doe',
    username: '1234567890',
    createdAt: '2023-10-01T12:00:00Z',
    isActive: true,
    roles: ['User'],

  });
  // dashboardService = inject(DashboardService);
  
  // portfolioChange = computed(() => this.dashboardService.portfolioChange());
  
/*   recentTransactions = computed(() => 
    this.dashboardService.transactionsData()
      .slice()
      .sort((a, b) => b.date.getTime() - a.date.getTime())
      .slice(0, 5)
  ); */
}