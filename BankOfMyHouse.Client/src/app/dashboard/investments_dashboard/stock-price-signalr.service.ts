import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { CompanyStockPrice } from '../../investments/models/listCompany/listCompanyResponseDto';

@Injectable({
  providedIn: 'root'
})
export class StockPriceSignalRService {
  private chartDataSignal = signal<CompanyStockPrice | null>(null);
  private allPricesSignal = signal<Map<number, CompanyStockPrice>>(new Map());
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

      // Listen for all prices dictionary (NEW optimized method)
      this.hubConnection.on('TransferAllPrices', (pricesDict: Record<number, CompanyStockPrice>) => {
        console.log('Received all prices from SignalR:', pricesDict);

        // Convert object to Map
        const pricesMap = new Map<number, CompanyStockPrice>();
        for (const [companyIdStr, priceData] of Object.entries(pricesDict)) {
          const companyId = Number(companyIdStr);
          pricesMap.set(companyId, priceData);
        }

        this.allPricesSignal.set(pricesMap);
        console.log('Updated prices map with', pricesMap.size, 'companies');
      });
      
      this.hubConnection.on('TransferChartData', (stockPrice: CompanyStockPrice) => {
        console.log(`Single stock price received: ${stockPrice.companyId}`);
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