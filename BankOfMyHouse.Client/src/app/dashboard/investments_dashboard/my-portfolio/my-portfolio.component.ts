import { Component, computed, effect, inject, input, output, OnDestroy, signal, ViewChild, ElementRef, AfterViewInit, OnInit, untracked } from '@angular/core';
import { Chart, LineElement, PointElement, LinearScale, CategoryScale, Title, Tooltip, Legend, ChartConfiguration } from 'chart.js';
import { CommonModule } from '@angular/common';
import { MatGridListModule } from "@angular/material/grid-list";
import { MatButtonModule } from "@angular/material/button";
import { MatButtonToggleModule } from "@angular/material/button-toggle";
import { MatCardModule } from "@angular/material/card";
import { MatIconModule } from "@angular/material/icon";
import { InvestmentService } from '../investment.service';
import { CompanyStockPrice } from '../../../investments/models/listCompany/listCompanyResponseDto';

Chart.register(LineElement, PointElement, LinearScale, CategoryScale, Title, Tooltip, Legend);

@Component({
  selector: 'app-my-portfolio',
  imports: [CommonModule, MatGridListModule, MatButtonModule, MatButtonToggleModule, MatCardModule, MatIconModule],
  templateUrl: './my-portfolio.component.html',
  styleUrl: './my-portfolio.component.css'
})
export class MyPortfolioComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('chartCanvas', { static: false }) chartCanvas!: ElementRef<HTMLCanvasElement>;

  // Inputs from parent
  readonly investments = input<any[]>([]);
  readonly currentPrices = input<Map<number, number>>(new Map());
  readonly selectedTimeRange = input<number>(12);
  readonly timeRanges = input<Array<{label: string, hours: number, type: 'hours' | 'day' | 'month' | 'year'}>>([]);

  // Outputs to parent
  readonly timeRangeChange = output<number>();
  readonly createInvestment = output<void>();

  // Internal state
  protected readonly isLoadingHistorical = signal(false);
  private readonly stockPriceHistory = signal<CompanyStockPrice[]>([]);

  private investmentService = inject(InvestmentService);
  private chart: Chart | null = null;

  // Computed signals for enhanced KPIs
  protected readonly investmentsWithKPI = computed(() => {
    const investments = this.investments();
    const currentPrices = this.currentPrices();

    return investments.map(investment => {
      const companyId = investment.company?.id;
      const currentPrice = companyId ? currentPrices.get(companyId) : null;
      const sharesAmount = investment.sharesAmount || 0;
      const currentValue = currentPrice ? currentPrice * sharesAmount : null;

      return {
        ...investment,
        currentStockPrice: currentPrice,
        currentValue: currentValue,
        formattedCurrentValue: currentValue ? `€${currentValue.toFixed(2)}` : 'N/A'
      };
    });
  });

  // Chart configuration for portfolio
  protected readonly chartConfig = computed<ChartConfiguration<'line'>>(() => {
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

  constructor() {
    // Effect to update chart when history changes
    effect(() => {
      const history = this.stockPriceHistory();

      untracked(() => {
        if (this.chart && history.length > 0) {
          try {
            const config = this.chartConfig();
            this.chart.data = config.data;
            this.chart.update('none');
          } catch (error) {
            console.error('Error updating portfolio chart:', error);
          }
        }
      });
    });
  }

  ngOnInit(): void {
    const initialRange = this.timeRanges().find(r => r.hours === this.selectedTimeRange());
    this.loadHistoricalData(this.selectedTimeRange(), initialRange?.type || 'hours');
  }

  ngAfterViewInit(): void {
    this.createChart();
  }

  ngOnDestroy(): void {
    if (this.chart) {
      this.chart.destroy();
    }
  }

  protected loadHistoricalData(hours: number, type: 'hours' | 'day' | 'month' | 'year' = 'hours'): void {
    this.isLoadingHistorical.set(true);

    const dateRange = this.calculateDateRange(type, hours);

    this.investmentService.getHistoricalPrices(dateRange.hours, undefined, dateRange.startDate, dateRange.endDate).subscribe({
      next: (response) => {
        // Flatten the dictionary of company prices into a single array
        const allPrices: CompanyStockPrice[] = [];

        for (const companyId in response.companyPrices) {
          const prices = response.companyPrices[companyId];
          allPrices.push(...prices);
        }

        this.stockPriceHistory.set(allPrices);
        this.isLoadingHistorical.set(false);
      },
      error: (error) => {
        console.error('Error loading historical data:', error);
        this.isLoadingHistorical.set(false);
      }
    });
  }

  protected onTimeRangeChange(hours: number): void {
    const range = this.timeRanges().find(r => r.hours === hours);
    this.timeRangeChange.emit(hours);
    this.loadHistoricalData(hours, range?.type || 'hours');
  }

  protected onCreateInvestment(): void {
    this.createInvestment.emit();
  }

  private calculateDateRange(type: 'hours' | 'day' | 'month' | 'year', hours: number): { startDate?: Date, endDate?: Date, hours?: number } {
    const now = new Date();

    switch (type) {
      case 'hours':
        return { hours: hours };
      case 'day':
        const startOfDay = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
        return { startDate: startOfDay, endDate: now };
      case 'month':
        const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0, 0);
        return { startDate: startOfMonth, endDate: now };
      case 'year':
        const startOfYear = new Date(now.getFullYear(), 0, 1, 0, 0, 0, 0);
        return { startDate: startOfYear, endDate: now };
    }
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
      console.log('Portfolio chart created successfully');
    } catch (error) {
      console.error('Error creating portfolio chart:', error);
    }
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
}
