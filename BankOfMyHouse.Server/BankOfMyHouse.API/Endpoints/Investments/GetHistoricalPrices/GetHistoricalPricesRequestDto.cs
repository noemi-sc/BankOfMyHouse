using Microsoft.AspNetCore.Mvc;

namespace BankOfMyHouse.API.Endpoints.Investments.GetHistoricalPrices
{
	public class GetHistoricalPricesRequestDto
	{
		[FromQuery]
		public int Hours { get; set; } = 12;

		[FromQuery]
		public DateTime? StartDate { get; set; }

		[FromQuery]
		public DateTime? EndDate { get; set; }

		[FromQuery]
		public int? CompanyId { get; set; }
	}
}
