using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;

namespace BankOfMyHouse.Application.Services.Accounts.Interfaces
{
	public interface IBankAccountService
	{
		Task<BankAccount> GenerateBankAccount(int userId);
		Task<ICollection<BankAccount>> GetBankAccounts(int id, CancellationToken ct);
		Task<Transaction> CreateTransaction(IbanCode sender, IbanCode receiver, decimal amount, CancellationToken ct);
		Task<IEnumerable<Transaction>> GetTransactions(int userId, string iban, CancellationToken ct, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null);
	}
}
