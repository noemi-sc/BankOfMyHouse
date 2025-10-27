namespace BankOfMyHouse.API.Endpoints.Investments.GetHistoricalPrices
{
	public class GetHistoricalPricesRequestDto
	{
		public int Hours { get; set; } = 12;
		public int? CompanyId { get; set; } // Optional: filter by specific company
	}
}
