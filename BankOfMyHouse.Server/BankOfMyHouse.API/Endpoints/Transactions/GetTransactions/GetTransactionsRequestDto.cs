using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Transactions.GetTransactions
{
	public sealed record GetTransactionsRequestDto
	{
		[FromQuery]
		public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow.AddDays(-7);
		[FromQuery]
		public DateTimeOffset EndDate { get; set; } = DateTimeOffset.UtcNow;
		[FromQuery]
		public string Iban { get; set; }
	}
}