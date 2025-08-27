using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.Infrastructure.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly BankOfMyHouseDbContext _context;

    public CurrencyRepository(BankOfMyHouseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Currencies
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<Currency?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Currencies.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Currencies.ToListAsync(cancellationToken);
    }

    public async Task<Currency> AddAsync(Currency currency, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Currencies.AddAsync(currency, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(Currency currency, CancellationToken cancellationToken = default)
    {
        _context.Currencies.Update(currency);
        await _context.SaveChangesAsync(cancellationToken);
    }
}