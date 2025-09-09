export class listCompanyResponseDto {
	companies: Company[] = [];
}

export class Company {
	id: number = 0;
	name: string = '';
	stockPriceHistory: CompanyStockPrice[] = [];
}

export class CompanyStockPrice {
	constructor(stockPrice: number = 0, companyId: number = 0) {
		this.stockPrice = Math.round(stockPrice * 1000) / 1000; // Round to 3 decimal places
		this.timeOfPriceChange = new Date();
		this.companyId = companyId;
	}

	stockPrice: number = 0;
	timeOfPriceChange: Date = new Date();
	companyId: number = 0;
}