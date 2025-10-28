import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { StockPriceDto } from '../../investments/models/dtos/stock-price.dto';

@Injectable({
  providedIn: 'root'
})
export class StockPriceSignalRService {
  private chartDataSignal = signal<StockPriceDto | null>(null);
  private allPricesSignal = signal<Map<number, StockPriceDto>>(new Map());
  public connectionStatus = signal<'disconnected' | 'connecting' | 'connected'>('disconnected');
  private hubConnection!: signalR.HubConnection;

  public readonly chartData = this.chartDataSignal.asReadonly();
  public readonly allPrices = this.allPricesSignal.asReadonly();

  public stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  public async startConnection(): Promise<void> {
    this.connectionStatus.set('connecting');

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:57460/personalInvestments', {
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information) // Add logging for debugging
      .build();

    try {
      await this.hubConnection.start();

      // Listen for all prices dictionary
      this.hubConnection.on('TransferAllPrices', (pricesDict: Record<number, StockPriceDto>) => {
        console.log('Received all prices from SignalR:', pricesDict);

        // Convert object to Map with date conversion
        const pricesMap = new Map<number, StockPriceDto>();
        for (const [companyIdStr, priceData] of Object.entries(pricesDict)) {
          const companyId = Number(companyIdStr);

          // Convert timeOfPriceChange string to Date if needed
          const stockPrice: StockPriceDto = {
            ...priceData,
            timeOfPriceChange: new Date(priceData.timeOfPriceChange)
          };

          pricesMap.set(companyId, stockPrice);
        }

        this.allPricesSignal.set(pricesMap);
        console.log('Updated prices map with', pricesMap.size, 'companies');
      });
      
      this.hubConnection.on('TransferChartData', (priceData: StockPriceDto) => {
        console.log(`Single stock price received: ${priceData.companyId}`);

        // Convert timeOfPriceChange string to Date if needed
        const stockPrice: StockPriceDto = {
          ...priceData,
          timeOfPriceChange: new Date(priceData.timeOfPriceChange)
        };

        this.chartDataSignal.set(stockPrice);
      });

      console.log('SignalR connection started');
      this.connectionStatus.set('connected');

    } catch (err) {
      console.log('Error while starting connection: ' + err);
      this.connectionStatus.set('disconnected');
    }
  }
}