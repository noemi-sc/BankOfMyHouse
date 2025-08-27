using BankOfMyHouse.Domain.BankAccounts;

namespace BankOfMyHouse.Domain.Repositories;

public interface ICurrencyRepository
{
    Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Currency?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Currency> AddAsync(Currency currency, CancellationToken cancellationToken = default);
    Task UpdateAsync(Currency currency, CancellationToken cancellationToken = default);
}