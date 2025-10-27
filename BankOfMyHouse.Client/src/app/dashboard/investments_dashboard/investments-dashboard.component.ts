import { Component, computed, effect, inject, OnDestroy, Signal, signal, ViewChild, ElementRef, AfterViewInit, OnInit, untracked } from '@angular/core';
import { Chart, LineElement, PointElement, LinearScale, CategoryScale, Title, Tooltip, Legend, ChartConfiguration, ChartData, ArcElement, PieController } from 'chart.js';
import { CommonModule } from '@angular/common';
import { StockPriceSignalRService } from './stock-price-signalr.service';
import { MatGridListModule } from "@angular/material/grid-list";
import { MatButtonModule } from "@angular/material/button";
import { MatButtonToggleModule } from "@angular/material/button-toggle";
import { MatCardModule } from "@angular/material/card";
import { MatTableModule } from "@angular/material/table";
import { MatTabsModule } from "@angular/material/tabs";
import { MatIconModule } from "@angular/material/icon";
import { MatChipsModule } from "@angular/material/chips";
import { CreateInvestmentComponent } from "../../investments/create-investment/create-investment.component";
import { InvestmentService } from './investment.service';
import { CompanyStockPrice } from '../../investments/models/listCompany/listCompanyResponseDto';

Chart.register(LineElement, PointElement, LinearScale, CategoryScale, Title, Tooltip, Legend, ArcElement, PieController);

@Component({
  selector: 'app-investments-dashboard',
  templateUrl: './investments-dashboard.component.html',
  styleUrl: './investments-dashboard.component.css',
  standalone: true,
  imports: [CommonModule, MatGridListModule, MatButtonModule, MatButtonToggleModule, MatCardModule, MatTableModule, MatTabsModule, MatIconModule, MatChipsModule, CreateInvestmentComponent]
})

export class InvestmentsDashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('chartCanvas', { static: false }) chartCanvas!: ElementRef<HTMLCanvasElement>;
  @ViewChild('portfolioChartCanvas', { static: false }) portfolioChartCanvas!: ElementRef<HTMLCanvasElement>;

  protected readonly isInvestmentPopupOpen = signal(false);
  protected readonly inputValue = signal('');
  protected readonly investments = signal<any[]>([]);
  protected readonly selectedTimeRange = signal<number>(12); // hours
  protected readonly isLoadingHistorical = signal(false);
  private readonly stockPriceHistory = signal<CompanyStockPrice[]>([]);
  private readonly currentStockPrices = signal<Map<number, number>>(new Map());
  protected readonly activeTab = signal<number>(0);
  protected readonly allCompanies = signal<any[]>([]);
  protected readonly isLoadingCompanies = signal(false);

  private stockPriceService = inject(StockPriceSignalRService);
  private investmentService = inject(InvestmentService);

  private readonly chartData = this.stockPriceService.chartData;
  private chart: Chart | null = null;
  private portfolioChart: Chart | null = null;

  // Computed signals for enhanced KPIs
  protected readonly investmentsWithKPI = computed(() => {
    const investments = this.investments();
    const currentPrices = this.currentStockPrices();

    return investments.map(investment => {
      const companyId = investment.company?.id;
      const currentPrice = companyId ? currentPrices.get(companyId) : null;
      const sharesAmount = investment.sharesAmount || 0;

      // We'll need to get the purchase price from historical data or investment details
      // For now, let's assume we need to implement this
      const currentValue = currentPrice ? currentPrice * sharesAmount : null;

      return {
        ...investment,
        currentStockPrice: currentPrice,
        currentValue: currentValue,
        formattedCurrentValue: currentValue ? `€${currentValue.toFixed(2)}` : 'N/A'
      };
    });
  });

  // Companies with enriched data for market discovery
  protected readonly companiesWithPrices = computed(() => {
    const companies = this.allCompanies();
    const currentPrices = this.currentStockPrices();
    const userInvestments = this.investments();

    return companies.map(company => {
      const currentPrice = currentPrices.get(company.id);
      const userInvestment = userInvestments.find(inv => inv.company?.id === company.id);
      const isInvested = !!userInvestment;

      return {
        ...company,
        currentPrice: currentPrice || 0,
        formattedPrice: currentPrice ? `€${currentPrice.toFixed(2)}` : 'N/A',
        isInvested,
        userShares: userInvestment?.sharesAmount || 0,
        userInvestmentValue: userInvestment && currentPrice ?
          (userInvestment.sharesAmount * currentPrice) : 0
      };
    });
  });

  // Time range options
  protected readonly timeRanges = [
    { label: '12h', hours: 12 },
    { label: '1d', hours: 24 },
    { label: '1m', hours: 24 * 30 },
    { label: '1y', hours: 24 * 365 }
  ];

  private readonly chartConfig = computed<ChartConfiguration<'line'>>(() => {
    const data = this.chartData();
    const history = this.stockPriceHistory();
    const userInvestments = this.investments();

    // Get the company IDs that the user has invested in
    const investedCompanyIds = new Set(userInvestments.map(inv => inv.company?.id).filter(id => id != null));

    // Group data by company ID and round timestamps to minutes
    const companiesData = new Map<number, Map<string, CompanyStockPrice>>();
    const minuteSet = new Set<string>();

    if (history.length > 0) {
      // Group historical data by company and minute, only for invested companies
      history.forEach(item => {
        const companyId = item.companyId;

        // Only include data for companies the user has invested in
        if (!investedCompanyIds.has(companyId)) {
          return;
        }

        if (!companiesData.has(companyId)) {
          companiesData.set(companyId, new Map());
        }

        // Round timestamp to the nearest minute
        const date = typeof item.timeOfPriceChange === 'string'
          ? new Date(item.timeOfPriceChange)
          : item.timeOfPriceChange;

        const roundedDate = new Date(date);
        roundedDate.setSeconds(0, 0); // Set seconds and milliseconds to 0

        const minuteLabel = roundedDate.toLocaleTimeString('it-IT', {
          hour: '2-digit',
          minute: '2-digit'
        });

        // Store the latest price for this minute
        companiesData.get(companyId)!.set(minuteLabel, item);
        minuteSet.add(minuteLabel);
      });
    } else if (data && investedCompanyIds.has(data.companyId)) {
      // Single data point, only if user invested in this company
      const companyId = data.companyId;
      companiesData.set(companyId, new Map());

      const date = typeof data.timeOfPriceChange === 'string'
        ? new Date(data.timeOfPriceChange)
        : data.timeOfPriceChange;

      const roundedDate = new Date(date);
      roundedDate.setSeconds(0, 0);

      const minuteLabel = roundedDate.toLocaleTimeString('it-IT', {
        hour: '2-digit',
        minute: '2-digit'
      });

      companiesData.get(companyId)!.set(minuteLabel, data);
      minuteSet.add(minuteLabel);
    }

    // Convert minute set to sorted array for labels
    const labels = Array.from(minuteSet).sort();

    // Generate colors for different companies
    const colors = [
      'rgb(75, 192, 192)', 'rgb(255, 99, 132)', 'rgb(54, 162, 235)',
      'rgb(255, 205, 86)', 'rgb(153, 102, 255)', 'rgb(255, 159, 64)',
      'rgb(199, 199, 199)', 'rgb(83, 102, 147)', 'rgb(255, 99, 255)',
      'rgb(99, 255, 132)'
    ];

    // Create datasets for each company, using company names from investments
    const datasets = Array.from(companiesData.entries()).map(([companyId, companySecondData], index) => {
      const color = colors[index % colors.length];

      // Find the company name from user investments
      const investment = userInvestments.find(inv => inv.company?.id === companyId);
      const companyName = investment?.company?.name || `Azienda ${companyId}`;

      // Create data array matching the labels timeline
      const dataPoints = labels.map(timeLabel => {
        const priceData = companySecondData.get(timeLabel);
        return priceData ? Number(priceData.stockPrice) : null;
      });

      return {
        label: companyName,
        data: dataPoints,
        borderColor: color,
        backgroundColor: color.replace('rgb', 'rgba').replace(')', ', 0.2)'),
        tension: 0.1,
        fill: false,
        spanGaps: true // Connect points even with null values
      };
    });

    // Fallback data if no real data or no investments
    if (datasets.length === 0) {
      return {
        type: 'line',
        data: {
          labels: ['Nessun dato'],
          datasets: [{
            label: 'Nessun investimento',
            data: [0],
            borderColor: 'rgb(128, 128, 128)',
            backgroundColor: 'rgba(128, 128, 128, 0.2)',
            tension: 0.1,
            fill: false
          }]
        },
        options: this.getChartOptions()
      };
    }

    return {
      type: 'line',
      data: {
        labels,
        datasets
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          title: {
            display: true,
            text: 'Andamento Prezzi Azioni - I miei investimenti'
          },
          legend: {
            display: true,
            position: 'top'
          }
        },
        scales: {
          y: {
            beginAtZero: false,
            title: {
              display: true,
              text: 'Prezzo (€)'
            }
          },
          x: {
            title: {
              display: true,
              text: 'Tempo'
            }
          }
        }
      }
    };
  });

  ngOnInit(): void {
    this.loadInvestments();
    this.loadAllCompanies();
    this.loadHistoricalData(this.selectedTimeRange());
  }

  constructor() {
    this.stockPriceService.startConnection();

    // Effect to accumulate stock price history and update current prices
    effect(() => {
      const newData = this.chartData();

      if (newData) {
        console.log('New SignalR data received:', newData);

        // Use untracked to read current values without creating dependencies
        untracked(() => {
          const currentHistory = this.stockPriceHistory();
          const currentPrices = this.currentStockPrices();

          // Update current stock prices map
          const newPrices = new Map(currentPrices);
          newPrices.set(newData.companyId, Number(newData.stockPrice));
          this.currentStockPrices.set(newPrices);

          // Check if this is actually new data (avoid duplicates)
          const isDuplicate = currentHistory.some(item => {
            const itemTime = typeof item.timeOfPriceChange === 'string'
              ? new Date(item.timeOfPriceChange)
              : item.timeOfPriceChange;
            const newTime = typeof newData.timeOfPriceChange === 'string'
              ? new Date(newData.timeOfPriceChange)
              : newData.timeOfPriceChange;

            return item.stockPrice === newData.stockPrice &&
              itemTime.getTime() === newTime.getTime() &&
              item.companyId === newData.companyId;
          });

          if (!isDuplicate) {
            const updatedHistory = [...currentHistory, newData];

            // Keep only last 20 data points
            if (updatedHistory.length > 20) {
              updatedHistory.shift();
            }

            console.log('Updating history with new data:', updatedHistory);
            this.stockPriceHistory.set(updatedHistory);
          }
        });
      }
    });

    // Separate effect to update chart when history changes
    effect(() => {
      const history = this.stockPriceHistory();

      console.log('Chart update triggered. History length:', history.length);

      // Use untracked to avoid creating dependencies on chart and investments
      untracked(() => {
        if (this.chart && history.length > 0) {
          try {
            const config = this.chartConfig();
            console.log('Updating chart with config:', config.data);

            this.chart.data = config.data;
            this.chart.update('none'); // Use 'none' for immediate update without animation
          } catch (error) {
            console.error('Error updating chart:', error);
          }
        }
      });
    });
  }

  ngAfterViewInit(): void {
    this.createChart();
  }

  ngOnDestroy(): void {
    if (this.chart) {
      this.chart.destroy();
    }
    this.stockPriceService.stopConnection();
  }

  protected loadInvestments(): void {
    this.investmentService.listInvestment().subscribe({
      next: (response) => {
        this.investments.set(response.investments || []);
        console.log('Investments loaded:', response);
      },
      error: (error) => {
        console.error('Error loading investments:', error);
      }
    });
  }

  protected loadAllCompanies(): void {
    this.isLoadingCompanies.set(true);
    this.investmentService.listCompanies().subscribe({
      next: (response) => {
        this.allCompanies.set(response.companies || []);
        this.isLoadingCompanies.set(false);
        console.log('Companies loaded:', response);
      },
      error: (error) => {
        console.error('Error loading companies:', error);
        this.isLoadingCompanies.set(false);
      }
    });
  }

  protected loadHistoricalData(hours: number): void {
    this.isLoadingHistorical.set(true);
    this.investmentService.getHistoricalPrices(hours).subscribe({
      next: (response) => {
        // Flatten the dictionary of company prices into a single array
        const allPrices: CompanyStockPrice[] = [];
        const currentPrices = new Map<number, number>();

        for (const companyId in response.companyPrices) {
          const prices = response.companyPrices[companyId];
          allPrices.push(...prices);

          // Get the latest price for each company
          if (prices.length > 0) {
            const latestPrice = prices[prices.length - 1];
            currentPrices.set(Number(companyId), Number(latestPrice.stockPrice));
          }
        }

        this.stockPriceHistory.set(allPrices);
        this.currentStockPrices.set(currentPrices);
        this.isLoadingHistorical.set(false);
        console.log('Historical data loaded:', allPrices.length, 'data points');
      },
      error: (error) => {
        console.error('Error loading historical data:', error);
        this.isLoadingHistorical.set(false);
      }
    });
  }

  protected onTimeRangeChange(hours: number): void {
    this.selectedTimeRange.set(hours);
    this.loadHistoricalData(hours);
  }

  protected switchTab(tabIndex: number): void {
    this.activeTab.set(tabIndex);
  }

  private getChartOptions() {
    return {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: true,
          text: 'Andamento Prezzi Azioni'
        },
        legend: {
          display: true,
          position: 'top' as const
        }
      },
      scales: {
        y: {
          beginAtZero: false,
          title: {
            display: true,
            text: 'Prezzo (€)'
          }
        },
        x: {
          title: {
            display: true,
            text: 'Tempo'
          }
        }
      }
    };
  }

  private createChart(): void {
    if (!this.chartCanvas) {
      console.error('Chart canvas not found');
      return;
    }

    const ctx = this.chartCanvas.nativeElement.getContext('2d');
    if (!ctx) {
      console.error('Unable to get canvas context');
      return;
    }

    // Destroy existing chart if it exists
    if (this.chart) {
      this.chart.destroy();
    }

    try {
      const config = this.chartConfig();
      this.chart = new Chart(ctx, config);
      console.log('Chart created successfully');
    } catch (error) {
      console.error('Error creating chart:', error);
    }
  }

  public async reconnect(): Promise<void> {
    try {
      this.stockPriceService.stopConnection();
      await this.stockPriceService.startConnection();
    } catch (error) {
      console.error('Failed to reconnect:', error);
    }
  }
  // Popup for new payment

  protected openInvestmentPopup(): void {
    this.isInvestmentPopupOpen.set(true);
  }

  protected onInvestmentPopupConfirmed(value: string): void {
    console.log('Transaction confirmed with value:', value);
    this.inputValue.set(value);
    this.isInvestmentPopupOpen.set(false);
  }

  protected onInvestmentPopupCancelled(): void {
    console.log('Transaction popup cancelled');
    this.isInvestmentPopupOpen.set(false);
  }
}
