namespace BankOfMyHouse.API.Endpoints.Transactions.CreateTransaction;

public sealed record CreateTransactionResponseDto
{
	public DateTimeOffset CreatedAt { get; set; }
}