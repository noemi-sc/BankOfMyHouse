using BankOfMyHouse.API.Endpoints.Transactions.DTOs;

namespace BankOfMyHouse.API.Endpoints.Transactions.GetTransactions;

public sealed record GetTransactionsResponseDto
{
	public List<TransactionDto> Transactions { get; set; } = new();
}