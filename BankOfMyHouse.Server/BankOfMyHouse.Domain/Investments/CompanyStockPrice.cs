namespace BankOfMyHouse.Domain.Investments
{
	public sealed record CompanyStockPrice
	{
		public CompanyStockPrice(decimal stockPrice, int companyId)
		{
			StockPrice = Math.Round(stockPrice, 3);
			TimeOfPriceChange = DateTimeOffset.UtcNow;
			CompanyId = companyId;
		}

		public decimal StockPrice { get; init; }
		public DateTimeOffset TimeOfPriceChange { get; init; }
		public int CompanyId { get; init; }
	}
}
