using BankOfMyHouse.API.Endpoints.Transactions.DTOs;
using BankOfMyHouse.Domain.BankAccounts;
using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Transactions.CreateTransaction;

public sealed record CreateTransactionRequestDto
{
	[FromClaim("userId")]
	public int UserId { get; set; }

	public required IbanCodeDto SenderIban { get; set; }
	public required IbanCodeDto ReceiverIban { get; set; }
	public required decimal Amount { get; set; }
	public required string CurrencyCode { get; set; }
	public PaymentCategoryCode PaymentCategory { get; set; } = PaymentCategoryCode.Other;
	public string? Description { get; set; }
}