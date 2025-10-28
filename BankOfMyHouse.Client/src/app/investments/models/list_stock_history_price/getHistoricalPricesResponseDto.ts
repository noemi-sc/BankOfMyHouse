import { StockPriceDto } from '../dtos/stock-price.dto';

export class GetHistoricalPricesResponseDto {
    companyPrices: Record<number, StockPriceDto[]>;

    constructor() {
        this.companyPrices = {};
    }
}
