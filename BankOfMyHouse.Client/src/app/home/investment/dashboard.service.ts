// signalr.service.ts
import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { CompanyStockPrice } from './models/investment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  public chartData = signal<CompanyStockPrice[]>([]);
  public connectionStatus = signal<'disconnected' | 'connecting' | 'connected'>('disconnected');
  private hubConnection!: signalR.HubConnection;

  public stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  public async startConnection(): Promise<void> {
    this.connectionStatus.set('connecting');

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:57460/personalInvestments', {
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information) // Add logging for debugging
      .build();

    try {
      await this.hubConnection.start();
      console.log('SignalR connection started');
      this.connectionStatus.set('connected');
      this.addTransferChartDataListener();
    } catch (err) {
      console.log('Error while starting connection: ' + err);
      this.connectionStatus.set('disconnected');
    }
  }

  private addTransferChartDataListener(): void {
    this.hubConnection.on('TransferChartData', (receivedData: any) => {
      console.log('Raw received data:', receivedData, 'Type:', typeof receivedData);

      try {
        const stockPrices = this.parseReceivedData(receivedData);

        if (stockPrices.length > 0) {
          this.chartData.update(currentData => {
            const newData = [...currentData, ...stockPrices];
            const maxPoints = 20;
            // Keep only last 50 points for better performance
            return newData.length > maxPoints ? newData.slice(-maxPoints) : newData;
          });
          console.log('Successfully processed stock prices:', stockPrices.length);
        } else {
          console.warn('No valid stock price data found');
        }
      } catch (error) {
        console.error('Error processing received data:', error);
        // this.lastError.set('Error processing received data');
      }
    });
  }

  private parseReceivedData(data: any): CompanyStockPrice[] {
    const stockPrices: CompanyStockPrice[] = [];

    try {
      if (Array.isArray(data)) {
        // Data is an array of stock prices
        console.log('Data is an array with length:', data.length);
        for (const item of data) {
          const stockPrice = this.convertToStockPrice(item);
          if (stockPrice) {
            stockPrices.push(stockPrice);
          }
        }
      } else if (data && typeof data === 'object') {
        // Data is a single stock price object
        console.log('Data is a single object:', data);
        const stockPrice = this.convertToStockPrice(data);
        if (stockPrice) {
          stockPrices.push(stockPrice);
        }
      } else if (typeof data === 'number') {
        // Data is just a number (stock price), create object with current time
        console.log('Data is a number (stock price):', data);
        const stockPrice: CompanyStockPrice = {
          TimeOfPriceChange: new Date(),
          StockPrice: data,
          CompanyId: 0 // Assuming CompanyId is not provided, set to 0 or handle as needed
        };
        stockPrices.push(stockPrice);
      } else {
        console.warn('Received unexpected data format:', data);
      }
    } catch (error) {
      console.error('Error parsing received data:', error);
    }

    return stockPrices;
  }

  private convertToStockPrice(item: any): CompanyStockPrice | null {
    try {
      if (typeof item === 'object' && item !== null) {
        // Handle different property names that might come from server
        const timeOfPriceChange = item.TimeOfPriceChange;

        const stockPrice = item.StockPrice;

        return {
          TimeOfPriceChange: new Date(timeOfPriceChange),
          StockPrice: Number(stockPrice) || 0,
          CompanyId: item.CompanyId || 0 // Assuming CompanyId is always present
        };
      }

      return null;
    } catch (error) {
      console.error('Error converting item to stock price:', item, error);
      return null;
    }
  }

  public clearData(): void {
    this.chartData.set([]);
  }
}