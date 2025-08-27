using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.Infrastructure.Repositories;

public class PaymentCategoryRepository : IPaymentCategoryRepository
{
    private readonly BankOfMyHouseDbContext _context;

    public PaymentCategoryRepository(BankOfMyHouseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PaymentCategory?> GetByCodeAsync(PaymentCategoryCode code, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentCategories
            .FirstOrDefaultAsync(pc => pc.Code == code, cancellationToken);
    }

    public async Task<PaymentCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentCategories.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<PaymentCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentCategories.ToListAsync(cancellationToken);
    }

    public async Task<PaymentCategory> AddAsync(PaymentCategory paymentCategory, CancellationToken cancellationToken = default)
    {
        var entry = await _context.PaymentCategories.AddAsync(paymentCategory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(PaymentCategory paymentCategory, CancellationToken cancellationToken = default)
    {
        _context.PaymentCategories.Update(paymentCategory);
        await _context.SaveChangesAsync(cancellationToken);
    }
}