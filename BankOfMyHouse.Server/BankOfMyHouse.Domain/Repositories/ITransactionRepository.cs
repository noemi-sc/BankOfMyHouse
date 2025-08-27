using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;

namespace BankOfMyHouse.Domain.Repositories;

public interface ITransactionRepository
{
	Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
	Task<IEnumerable<Transaction>> GetBySenderIbanAsync(IbanCode iban, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, CancellationToken cancellationToken = default);
	Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}