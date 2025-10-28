/**
 * DTO for real-time stock price updates via SignalR.
 * Matches the backend StockPriceDto structure.
 * Property names are in camelCase as they are serialized from backend PascalCase.
 */
export interface StockPriceDto {
  stockPrice: number;
  timeOfPriceChange: string | Date;
  companyId: number;
}
