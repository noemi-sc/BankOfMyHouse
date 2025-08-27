using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;

namespace BankOfMyHouse.Domain.Repositories;

public interface IBankAccountRepository
{
	Task<BankAccount?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<BankAccount?> GetByIbanAsync(IbanCode iban, CancellationToken cancellationToken = default);
	Task<BankAccount?> GetByIbanWithUserAsync(IbanCode iban, CancellationToken cancellationToken = default);
	Task<IEnumerable<BankAccount>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
	Task<bool> IbanExistsAsync(IbanCode iban, CancellationToken cancellationToken = default);
	Task<BankAccount> AddAsync(BankAccount bankAccount, CancellationToken cancellationToken = default);
	Task UpdateAsync(BankAccount bankAccount, CancellationToken cancellationToken = default);
	Task UpdateRangeAsync(IEnumerable<BankAccount> bankAccounts, CancellationToken cancellationToken = default);
}