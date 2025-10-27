namespace BankOfMyHouse.API.Endpoints.Investments.GetHistoricalPrices
{
	public class GetHistoricalPricesResponseDto
	{
		public Dictionary<int, List<CompanyStockPriceDto>> CompanyPrices { get; set; } = new();
	}

	public class CompanyStockPriceDto
	{
		public decimal StockPrice { get; set; }
		public DateTimeOffset TimeOfPriceChange { get; set; }
		public int CompanyId { get; set; }
	}
}
