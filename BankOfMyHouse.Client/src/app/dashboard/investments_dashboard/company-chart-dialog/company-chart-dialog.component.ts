import { Component, inject, OnInit, ViewChild, ElementRef, signal, computed, effect, untracked, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { Chart, LineElement, PointElement, LinearScale, CategoryScale, Title, Tooltip, Legend, ChartConfiguration } from 'chart.js';
import { InvestmentService } from '../investment.service';
import { CompanyStockPrice } from '../../../investments/models/listCompany/listCompanyResponseDto';
import { StockPriceSignalRService } from '../stock-price-signalr.service';

Chart.register(LineElement, PointElement, LinearScale, CategoryScale, Title, Tooltip, Legend);

export interface CompanyChartDialogData {
  company: any;
}

@Component({
  selector: 'app-company-chart-dialog',
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatButtonToggleModule],
  templateUrl: './company-chart-dialog.component.html',
  styleUrl: './company-chart-dialog.component.css'
})
export class CompanyChartDialogComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('chartCanvas', { static: false }) chartCanvas!: ElementRef<HTMLCanvasElement>;

  private dialogRef = inject(MatDialogRef<CompanyChartDialogComponent>);
  protected data = inject<CompanyChartDialogData>(MAT_DIALOG_DATA);
  private investmentService = inject(InvestmentService);
  private stockPriceService = inject(StockPriceSignalRService);

  private chart: Chart | null = null;
  protected readonly companyTimeRange = signal<number>(12);
  protected readonly isLoadingHistorical = signal(false);
  private readonly companyChartData = signal<CompanyStockPrice[]>([]);
  protected readonly averagePrice = signal<number | null>(null);
  protected readonly liveCurrentPrice = signal<number | null>(null);

  protected readonly timeRanges = [
    { label: '12h', hours: 12, type: 'hours' as const },
    { label: '1d', hours: 24, type: 'day' as const },
    { label: '1m', hours: 24 * 30, type: 'month' as const },
    { label: '1y', hours: 24 * 365, type: 'year' as const }
  ];

  // Computed signal to calculate average price from chart data
  protected readonly computedAveragePrice = computed(() => {
    const history = this.companyChartData();

    if (history.length === 0) {
      return null;
    }

    const sum = history.reduce((acc, item) => acc + Number(item.stockPrice), 0);
    const avg = sum / history.length;
    console.log('Average price calculated:', avg, 'from', history.length, 'data points');
    return avg;
  });

  // Helper computed for current time range label
  protected readonly currentTimeRangeLabel = computed(() => {
    const hours = this.companyTimeRange();
    const range = this.timeRanges.find(r => r.hours === hours);
    return range?.label || '12h';
  });

  protected readonly chartConfig = computed<ChartConfiguration<'line'>>(() => {
    const history = this.companyChartData();
    const company = this.data.company;
    const timeRangeHours = this.companyTimeRange();

    if (history.length === 0) {
      return this.getEmptyChartConfig();
    }

    // Determine grouping granularity based on time range
    let groupingMinutes: number;
    if (timeRangeHours <= 24) {
      groupingMinutes = 1; // Group by minute for 12h and 24h
    } else if (timeRangeHours <= 24 * 7) {
      groupingMinutes = 15; // Group by 15 minutes for 1 week
    } else if (timeRangeHours <= 24 * 30) {
      groupingMinutes = 60; // Group by hour for 1 month
    } else {
      groupingMinutes = 60 * 24; // Group by day for 1 year
    }

    // Group by appropriate time interval
    const dataMap = new Map<number, CompanyStockPrice>();
    history.forEach(item => {
      const date = typeof item.timeOfPriceChange === 'string'
        ? new Date(item.timeOfPriceChange)
        : item.timeOfPriceChange;

      const roundedDate = new Date(date);
      const minutes = roundedDate.getMinutes();
      const roundedMinutes = Math.floor(minutes / groupingMinutes) * groupingMinutes;
      roundedDate.setMinutes(roundedMinutes, 0, 0);

      const timestamp = roundedDate.getTime();

      if (!dataMap.has(timestamp) ||
          new Date(item.timeOfPriceChange) > new Date(dataMap.get(timestamp)!.timeOfPriceChange)) {
        dataMap.set(timestamp, item);
      }
    });

    const sortedTimestamps = Array.from(dataMap.keys()).sort((a, b) => a - b);

    const labels = sortedTimestamps.map(timestamp => {
      const date = new Date(timestamp);
      if (timeRangeHours > 24) {
        return date.toLocaleDateString('it-IT', {
          day: '2-digit',
          month: '2-digit',
          hour: '2-digit',
          minute: '2-digit'
        });
      } else {
        return date.toLocaleTimeString('it-IT', {
          hour: '2-digit',
          minute: '2-digit'
        });
      }
    });

    const dataPoints = sortedTimestamps.map(timestamp => {
      const priceData = dataMap.get(timestamp);
      return priceData ? Number(priceData.stockPrice) : null;
    });

    const validPrices = dataPoints.filter(p => p !== null) as number[];
    const minPrice = Math.min(...validPrices);
    const maxPrice = Math.max(...validPrices);
    const priceRange = maxPrice - minPrice;

    const paddedMin = priceRange === 0 ? minPrice * 0.95 : minPrice - (priceRange * 0.1);
    const paddedMax = priceRange === 0 ? maxPrice * 1.05 : maxPrice + (priceRange * 0.1);

    return {
      type: 'line',
      data: {
        labels,
        datasets: [{
          label: company.name,
          data: dataPoints,
          borderColor: 'rgb(244, 63, 94)',
          backgroundColor: 'rgba(244, 63, 94, 0.1)',
          tension: 0.4,
          fill: true,
          pointRadius: 3,
          pointHoverRadius: 6,
          borderWidth: 2
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        animation: {
          duration: 750,
          easing: 'easeInOutQuart'
        },
        transitions: {
          active: {
            animation: {
              duration: 400
            }
          }
        },
        layout: {
          padding: {
            left: 10,
            right: 10,
            top: 10,
            bottom: 10
          }
        },
        plugins: {
          title: {
            display: false
          },
          legend: {
            display: false
          },
          tooltip: {
            mode: 'index',
            intersect: false
          }
        },
        scales: {
          y: {
            beginAtZero: false,
            min: paddedMin,
            max: paddedMax,
            ticks: {
              callback: function(value) {
                return '€' + Number(value).toFixed(2);
              }
            },
            grid: {
              display: true,
              drawOnChartArea: true,
              drawTicks: true
            }
          },
          x: {
            ticks: {
              maxRotation: 45,
              minRotation: 45,
              autoSkip: true,
              maxTicksLimit: 15
            },
            grid: {
              display: true,
              drawOnChartArea: true,
              drawTicks: true
            }
          }
        }
      }
    };
  });

  constructor() {
    // Initialize live current price with the initial value
    this.liveCurrentPrice.set(this.data.company.currentPrice || null);

    // Effect to update chart when data changes
    effect(() => {
      const config = this.chartConfig();

      untracked(() => {
        if (this.chart) {
          this.chart.data = config.data;
          if (config.options) {
            this.chart.options = config.options;
          }
          this.chart.update('active'); // Use 'active' for smooth transitions
        }
      });
    });

    // Effect to listen to SignalR price updates for this specific company
    effect(() => {
      const allPricesFromSignalR = this.stockPriceService.allPrices();
      const companyId = this.data.company.id;

      if (allPricesFromSignalR && allPricesFromSignalR.size > 0) {
        const priceData = allPricesFromSignalR.get(companyId);
        if (priceData) {
          const newPrice = Number(priceData.stockPrice);
          this.liveCurrentPrice.set(newPrice);
          console.log(`Updated live price for company ${companyId}:`, newPrice);
        }
      }
    });
  }

  ngOnInit(): void {
    this.loadChartData(12, 'hours');
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.createChart(), 100);
  }

  ngOnDestroy(): void {
    if (this.chart) {
      this.chart.destroy();
    }
  }

  protected onTimeRangeChange(hours: number): void {
    this.companyTimeRange.set(hours);
    const range = this.timeRanges.find(r => r.hours === hours);
    this.loadChartData(hours, range?.type || 'hours');
  }

  private loadChartData(hours: number, type: 'hours' | 'day' | 'month' | 'year'): void {
    this.isLoadingHistorical.set(true);
    const dateRange = this.calculateDateRange(type, hours);

    this.investmentService.getHistoricalPrices(
      dateRange.hours,
      this.data.company.id,
      dateRange.startDate,
      dateRange.endDate
    ).subscribe({
      next: (response) => {
        const allPrices: CompanyStockPrice[] = [];
        for (const cId in response.companyPrices) {
          const prices = response.companyPrices[cId];
          allPrices.push(...prices);
        }
        this.companyChartData.set(allPrices);
        this.isLoadingHistorical.set(false);
      },
      error: (error) => {
        console.error('Error loading historical data:', error);
        this.isLoadingHistorical.set(false);
        this.companyChartData.set([]);
      }
    });
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

    if (this.chart) {
      this.chart.destroy();
    }

    try {
      const config = this.chartConfig();
      this.chart = new Chart(ctx, config);
    } catch (error) {
      console.error('Error creating chart:', error);
    }
  }

  private getEmptyChartConfig(): ChartConfiguration<'line'> {
    return {
      type: 'line',
      data: {
        labels: ['Caricamento...'],
        datasets: [{
          label: 'Nessun dato',
          data: [0],
          borderColor: 'rgba(128, 128, 128, 0.5)',
          backgroundColor: 'rgba(128, 128, 128, 0.1)',
          tension: 0.1
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: false
          }
        }
      }
    };
  }

  protected close(): void {
    this.dialogRef.close();
  }
}
