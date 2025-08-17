namespace BankOfMyHouse.API.Endpoints.Investments
{
	public class CreateInvestmentRequestDto
	{
		public int CompanyId { get; set; }
		public int BankAccountId { get; set; }
		public decimal InvestmentAmount { get; set; }
	}
}