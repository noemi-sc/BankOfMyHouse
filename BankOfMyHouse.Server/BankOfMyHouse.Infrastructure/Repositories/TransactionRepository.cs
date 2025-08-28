using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;
using BankOfMyHouse.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
	private readonly BankOfMyHouseDbContext _context;

	public TransactionRepository(BankOfMyHouseDbContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}

	public async Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
	{
		_context.Transactions.Add(transaction);
		await _context.SaveChangesAsync(cancellationToken);
		return transaction;
	}

	public async Task<IEnumerable<Transaction>> GetBySenderIbanAsync(IbanCode iban, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, CancellationToken cancellationToken = default)
	{
		var query = _context.Transactions
			.Include(t => t.Currency)
			.Include(t => t.PaymentCategory)
			.Where(x => x.Sender == iban);

		if (startDate.HasValue)
		{
			query = query.Where(x => x.TransactionCreation >= startDate.Value);
		}
		if (endDate.HasValue)
		{
			query = query.Where(x => x.TransactionCreation <= endDate.Value);
		}

		return await query.ToListAsync(cancellationToken);
	}

	public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _context.Transactions
			.Include(t => t.Currency)
			.Include(t => t.PaymentCategory)
			.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
	}
}