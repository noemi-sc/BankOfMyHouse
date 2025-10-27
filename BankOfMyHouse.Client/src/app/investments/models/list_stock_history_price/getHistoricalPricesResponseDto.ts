export class CompanyStockPriceDto {
    stockPrice: number;
    timeOfPriceChange: Date;
    companyId: number;

    constructor(stockPrice: number, timeOfPriceChange: Date, companyId: number) {
        this.stockPrice = stockPrice;
        this.timeOfPriceChange = timeOfPriceChange;
        this.companyId = companyId;
    }
}

export class GetHistoricalPricesResponseDto {
    companyPrices: Record<number, CompanyStockPriceDto[]>;

    constructor() {
        this.companyPrices = {};
    }
}
