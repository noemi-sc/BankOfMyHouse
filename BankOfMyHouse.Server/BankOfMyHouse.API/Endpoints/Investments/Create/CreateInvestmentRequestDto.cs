namespace BankOfMyHouse.API.Endpoints.Investments.Create;

public class CreateInvestmentRequestDto
{
	public int CompanyId { get; set; }
	public int BankAccountId { get; set; }
	public decimal InvestmentAmount { get; set; }
}