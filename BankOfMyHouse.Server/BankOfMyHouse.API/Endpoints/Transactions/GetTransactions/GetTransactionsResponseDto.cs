using BankOfMyHouse.Domain.BankAccounts;

namespace BankOfMyHouse.API.Endpoints.Transactions.GetTransactions;

public sealed record GetTransactionsResponseDto
{
	public IEnumerable<Transaction> Transactions { get; set; }
}