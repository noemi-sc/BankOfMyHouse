using BankOfMyHouse.Domain.BankAccounts;

namespace BankOfMyHouse.Domain.Repositories;

public interface IPaymentCategoryRepository
{
    Task<PaymentCategory?> GetByCodeAsync(PaymentCategoryCode code, CancellationToken cancellationToken = default);
    Task<PaymentCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PaymentCategory> AddAsync(PaymentCategory paymentCategory, CancellationToken cancellationToken = default);
    Task UpdateAsync(PaymentCategory paymentCategory, CancellationToken cancellationToken = default);
}