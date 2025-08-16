namespace BankOfMyHouse.API.Endpoints.Transactions.GetTransactions
{
	public sealed record GetTransactionsRequestDto
	{
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
		public string Iban { get; set; }
	}
}