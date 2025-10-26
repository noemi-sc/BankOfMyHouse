using BankOfMyHouse.Domain.Investments;

namespace BankOfMyHouse.API.Endpoints.Investments.Get
{
	public class GetInvestmentsResponseDto
	{
		public List<Investment> Investments { get; set; }
	}
}