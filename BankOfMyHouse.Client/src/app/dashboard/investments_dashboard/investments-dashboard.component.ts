import { Component, computed, effect, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StockPriceSignalRService } from './stock-price-signalr.service';
import { MatTabsModule } from "@angular/material/tabs";
import { MatDialog } from "@angular/material/dialog";
import { InvestmentService } from './investment.service';
import { CompanyChartDialogComponent } from './company-chart-dialog/company-chart-dialog.component';
import { MyPortfolioComponent } from './my-portfolio/my-portfolio.component';
import { CompanyDiscoveryComponent } from './company-discovery/company-discovery.component';

@Component({
  selector: 'app-investments-dashboard',
  templateUrl: './investments-dashboard.component.html',
  styleUrl: './investments-dashboard.component.css',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    MyPortfolioComponent,
    CompanyDiscoveryComponent
  ]
})
export class InvestmentsDashboardComponent implements OnInit, OnDestroy {
  // Core state
  protected readonly investments = signal<any[]>([]);
  protected readonly allCompanies = signal<any[]>([]);
  protected readonly selectedTimeRange = signal<number>(12);
  protected readonly activeTab = signal<number>(0);
  protected searchQuery = signal<string>('');
  protected readonly companies24hAverages = signal<Map<number, number>>(new Map());
  protected readonly currentStockPrices = signal<Map<number, number>>(new Map());

  // Services
  private stockPriceService = inject(StockPriceSignalRService);
  private investmentService = inject(InvestmentService);
  private dialog = inject(MatDialog);

  // Time range options
  protected readonly timeRanges = [
    { label: '12h', hours: 12, type: 'hours' as const },
    { label: '1d', hours: 24, type: 'day' as const },
    { label: '1m', hours: 24 * 30, type: 'month' as const },
    { label: '1y', hours: 24 * 365, type: 'year' as const }
  ];

  // Computed: Investments enriched with full company details
  protected readonly enrichedInvestments = computed(() => {
    const investments = this.investments();
    const companies = this.allCompanies();

    return investments.map(investment => {
      // Find the company by companyId
      const company = companies.find(c => c.id === investment.companyId);

      return {
        ...investment,
        company: company || null
      };
    });
  });

  // Computed: Companies with enriched data (prices, trends, investment status)
  protected readonly companiesBaseData = computed(() => {
    const companies = this.allCompanies();
    const userInvestments = this.investments();
    const prices = this.currentStockPrices();
    const averages24h = this.companies24hAverages();
    const search = this.searchQuery().toLowerCase().trim();

    const enrichedCompanies = companies.map(company => {
      const userInvestment = userInvestments.find(inv => inv.companyId === company.id);
      const isInvested = !!userInvestment;
      const userShares = userInvestment?.sharesAmount || 0;
      const currentPrice = prices.get(company.id) || 0;
      const avg24h = averages24h.get(company.id) || 0;

      // Calculate percentage change vs 24h average
      let percentageChange = 0;
      let trend: 'up' | 'down' | 'neutral' = 'neutral';
      if (avg24h > 0 && currentPrice > 0) {
        percentageChange = ((currentPrice - avg24h) / avg24h) * 100;
        trend = percentageChange > 0 ? 'up' : percentageChange < 0 ? 'down' : 'neutral';
      }

      return {
        ...company,
        hasInvestment: isInvested,
        userShares,
        currentPrice,
        avg24h,
        percentageChange,
        trend,
        formattedPrice: currentPrice ? `€${currentPrice.toFixed(2)}` : 'N/A',
        investmentValue: userShares && currentPrice ? (userShares * currentPrice) : 0
      };
    });

    // Apply search filter
    if (search) {
      return enrichedCompanies.filter(company =>
        company.name.toLowerCase().includes(search) ||
        (company.symbol && company.symbol.toLowerCase().includes(search))
      );
    }

    return enrichedCompanies;
  });

  constructor() {
    this.stockPriceService.startConnection();

    // Effect to handle bulk price updates from SignalR
    effect(() => {
      const allPricesFromSignalR = this.stockPriceService.allPrices();

      if (allPricesFromSignalR && allPricesFromSignalR.size > 0) {
        // Extract prices from StockPriceDto objects
        const pricesMap = new Map<number, number>();
        for (const [companyId, priceData] of allPricesFromSignalR.entries()) {
          pricesMap.set(companyId, Number(priceData.stockPrice));
        }

        this.currentStockPrices.set(pricesMap);
      }
    });
  }

  ngOnInit(): void {
    this.loadInvestments();
    this.loadAllCompanies();
    this.load24hAverages();
  }

  ngOnDestroy(): void {
    this.stockPriceService.stopConnection();
  }

  // Data loading methods
  private loadInvestments(): void {
    this.investmentService.listInvestment().subscribe({
      next: (response: any) => {
        this.investments.set(response.investments || []);
      },
      error: (error: any) => console.error('Error loading investments:', error)
    });
  }

  private loadAllCompanies(): void {
    this.investmentService.listCompanies().subscribe({
      next: (response: any) => {
        this.allCompanies.set(response.companies || []);
      },
      error: (error: any) => console.error('Error loading companies:', error)
    });
  }

  private load24hAverages(): void {
    this.investmentService.getHistoricalPrices(24, undefined, undefined, undefined).subscribe({
      next: (response: any) => {
        const averagesMap = new Map<number, number>();

        for (const companyId in response.companyPrices) {
          const prices = response.companyPrices[companyId];
          if (prices.length > 0) {
            const sum = prices.reduce((acc: number, item: any) => acc + Number(item.stockPrice), 0);
            const avg = sum / prices.length;
            averagesMap.set(Number(companyId), avg);
          }
        }

        this.companies24hAverages.set(averagesMap);
      },
      error: (error: any) => console.error('Error loading 24h averages:', error)
    });
  }

  // Event handlers from child components
  protected onTimeRangeChange(hours: number): void {
    this.selectedTimeRange.set(hours);
  }

  protected onSearchQueryChange(query: string): void {
    this.searchQuery.set(query);
  }

  protected onViewCompanyChart(company: any): void {
    this.dialog.open(CompanyChartDialogComponent, {
      width: '900px',
      maxWidth: '95vw',
      data: { company }
    });
  }

  protected onInvestmentCreated(): void {
    // Reload investments after creation
    this.loadInvestments();
  }
}
