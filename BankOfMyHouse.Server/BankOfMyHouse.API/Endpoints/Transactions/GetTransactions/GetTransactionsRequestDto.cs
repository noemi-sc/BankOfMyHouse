using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Transactions.GetTransactions
{
	public sealed record GetTransactionsRequestDto
	{
		[FromClaim]
		public required string UserId { get; set; }

		public DateTimeOffset? StartDate { get; set; }

		public DateTimeOffset? EndDate { get; set; }

		public required string Iban { get; set; }
	}
}