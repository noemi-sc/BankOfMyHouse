using BankOfMyHouse.Domain.BankAccounts;

namespace BankOfMyHouse.API.Endpoints.Transactions.DTOs;

public sealed record TransactionDto
{
	public Guid Id { get; set; }
	public decimal Amount { get; set; }
	public DateTimeOffset TransactionCreation { get; set; }
	public PaymentCategoryCode PaymentCategoryCode { get; set; }
	public string PaymentCategoryName { get; set; } = string.Empty;
	public string CurrencyCode { get; set; } = string.Empty;
	public string CurrencySymbol { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string SenderIban { get; set; } = string.Empty;
	public string ReceiverIban { get; set; } = string.Empty;
}