import { StockPriceDto } from '../dtos/stock-price.dto';

export class listCompanyResponseDto {
	companies: Company[] = [];
}

export class Company {
	id: number = 0;
	name: string = '';
	stockPriceHistory: StockPriceDto[] = [];
}