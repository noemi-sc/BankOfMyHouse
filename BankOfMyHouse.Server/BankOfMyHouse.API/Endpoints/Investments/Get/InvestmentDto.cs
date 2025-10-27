namespace BankOfMyHouse.API.Endpoints.Investments.Get
{
	public class InvestmentDto
	{
		public int Id { get; set; }
		public decimal SharesAmount { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public int CompanyId { get; set; }
		public int BankAccountId { get; set; }
	}
}