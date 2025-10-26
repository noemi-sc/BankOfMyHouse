using BankOfMyHouse.Domain.Investments;

namespace BankOfMyHouse.API.Endpoints.Investments.ListCompanies
{
	public sealed record ListCompaniesResponseDto
	{
		public List<Company> Companies { get; set; }
	}
}