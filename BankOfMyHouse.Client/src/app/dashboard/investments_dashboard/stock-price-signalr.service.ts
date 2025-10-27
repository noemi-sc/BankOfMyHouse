import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { CompanyStockPrice } from '../../investments/models/listCompany/listCompanyResponseDto';

@Injectable({
  providedIn: 'root'
})
export class StockPriceSignalRService {
  private chartDataSignal = signal<CompanyStockPrice | null>(null);
  public connectionStatus = signal<'disconnected' | 'connecting' | 'connected'>('disconnected');
  private hubConnection!: signalR.HubConnection;

  public readonly chartData = this.chartDataSignal.asReadonly();

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

      this.hubConnection.on('TransferChartData', (stockPrice: CompanyStockPrice) => {
        console.log(`Stockprice: ${stockPrice.companyId})`);
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