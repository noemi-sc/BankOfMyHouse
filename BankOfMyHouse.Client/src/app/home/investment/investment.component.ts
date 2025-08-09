import { Component, computed, effect, OnDestroy, OnInit } from '@angular/core';
import { ChartType, ChartConfiguration } from 'chart.js';
import { SignalrService } from './dashboard.service';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { CompanyStockPrice } from './models/investment';

@Component({
  selector: 'app-investment',
  templateUrl: './investment.component.html',
  styleUrl: './investment.component.css',
  standalone: true,
  imports: [BaseChartDirective, CommonModule]
})
export class InvestmentComponent implements OnInit, OnDestroy {
  public chartType: ChartType = 'line';

  // Computed signals
  public dataPointCount = computed(() => {
    const data = this.signalrService.chartData();
    return Array.isArray(data) ? data.length : 0;
  });

  public lastUpdate = computed(() => {
    const data = this.signalrService.chartData();
    if (Array.isArray(data) && data.length > 0) {
      return data[data.length - 1].TimeOfPriceChange;
    }
    return null;
  });

  public latestPrice = computed(() => {
    const data = this.signalrService.chartData();
    if (Array.isArray(data) && data.length > 0) {
      return data[data.length - 1].StockPrice;
    }
    return null;
  });

  public connectionStatusClass = computed(() => {
    return this.signalrService.connectionStatus();
  });

  // Chart configuration
  public chartConfiguration = computed((): ChartConfiguration['data'] => {
    const data = this.signalrService.chartData();

    if (!Array.isArray(data)) {
      console.error('chartData is not an array:', data);
      return {
        labels: [],
        datasets: [{
          label: 'Stock Price',
          data: [],
          borderColor: 'rgb(75, 192, 192)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          fill: false,
          tension: 0.1
        }]
      };
    }

    console.log('Chart data:', data);

    return {
      labels: data.map(item => item.TimeOfPriceChange),
      datasets: [{
        label: 'Stock Price ($)',
        data: data.map(item => item.StockPrice),
        borderColor: 'rgb(54, 162, 235)',
        backgroundColor: 'rgba(54, 162, 235, 0.1)',
        borderWidth: 2,
        fill: false,
        tension: 0.1,
        pointRadius: 4,
        pointHoverRadius: 6,
        pointBackgroundColor: 'rgb(54, 162, 235)',
        pointBorderColor: '#fff',
        pointBorderWidth: 2
      }]
    };
  });

  public chartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    interaction: {
      intersect: false,
      mode: 'index'
    },
    scales: {
      y: {
        beginAtZero: false,
        title: {
          display: true,
          text: 'Stock Price ($)'
        },
        ticks: {
          callback: function (value) {
            return '$' + Number(value).toFixed(2);
          }
        }
      },
      x: {
        type: 'category',
        title: {
          display: true,
          text: 'Time'
        },
        ticks: {
          maxTicksLimit: 8
          // 🔧 REMOVED: Complex callback that was causing the error
        }
      }
    },
    plugins: {
      legend: {
        display: true,
        position: 'top'
      },
      tooltip: {
        callbacks: {
          title: function (context) {
            // 🔧 SIMPLIFIED: Just convert to string safely
            return String(context[0].label);
          },
          label: function (context) {
            return `Stock Price: $${Number(context.parsed.y).toFixed(4)}`;
          }
        }
      }
    },
    animation: {
      duration: 0
    }
  };

  public debugInfo = computed(() => {
    const data = this.signalrService.chartData();
    return {
      connectionStatus: this.signalrService.connectionStatus(),
      dataType: Array.isArray(data) ? 'array' : typeof data,
      dataLength: Array.isArray(data) ? data.length : 'N/A',
      hasData: Array.isArray(data) && data.length > 0,
      sampleData: Array.isArray(data) ? data.slice(-3) : data,
      chartVisible: Array.isArray(data) && data.length > 0
    };
  });

  constructor(public signalrService: SignalrService) {
    effect(() => {
      const status = this.signalrService.connectionStatus();
      console.log('Connection status changed:', status);
    });

    effect(() => {
      const dataCount = this.dataPointCount();
      console.log('Data points updated:', dataCount);
    });
  }

  async ngOnInit(): Promise<void> {
    try {
      await this.signalrService.startConnection();
    } catch (error) {
      console.error('Failed to start SignalR connection:', error);
    }
  }

  ngOnDestroy(): void {
    this.signalrService.stopConnection();
  }

  public async reconnect(): Promise<void> {
    try {
      this.signalrService.stopConnection();
      await this.signalrService.startConnection();
    } catch (error) {
      console.error('Failed to reconnect:', error);
    }
  }

  public clearChart(): void {
    this.signalrService.clearData();
  }
}