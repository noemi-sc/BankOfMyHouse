namespace BankOfMyHouse.API.Endpoints.Investments.Get;

public class GetInvestmentRequestDto
{
	public int CompanyId { get; set; }
	public int BankAccountId { get; set; }
	public decimal InvestmentAmount { get; set; }
}