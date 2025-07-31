using BankOfMyHouse.API.Endpoints.Transactions.DTOs;

namespace BankOfMyHouse.API.Endpoints.Transactions.CreateTransaction;

public sealed record CreateTransactionRequestDto
{
	public required IbanCodeDto SenderIban { get; set; }
	public required IbanCodeDto ReceiverIban { get; set; }
	public required decimal Amount { get; set; }
}