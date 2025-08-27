using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;
using BankOfMyHouse.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.Infrastructure.Repositories;

public class BankAccountRepository : IBankAccountRepository
{
	private readonly BankOfMyHouseDbContext _context;

	public BankAccountRepository(BankOfMyHouseDbContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}

	public async Task<BankAccount?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		return await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.Id == id, cancellationToken);
	}

	public async Task<BankAccount?> GetByIbanAsync(IbanCode iban, CancellationToken cancellationToken = default)
	{
		return await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.IBAN.Value == iban.Value, cancellationToken);
	}

	public async Task<BankAccount?> GetByIbanWithUserAsync(IbanCode iban, CancellationToken cancellationToken = default)
	{
		return await _context.BankAccounts
			.Include(ba => ba.User)
			.FirstOrDefaultAsync(ba => ba.IBAN.Value == iban.Value, cancellationToken);
	}

	public async Task<IEnumerable<BankAccount>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		return await _context.BankAccounts.Where(ba => ba.UserId == userId).ToListAsync(cancellationToken);
	}

	public async Task<bool> IbanExistsAsync(IbanCode iban, CancellationToken cancellationToken = default)
	{
		return await _context.BankAccounts.AnyAsync(ba => ba.IBAN.Value == iban.Value, cancellationToken);
	}

	public async Task<BankAccount> AddAsync(BankAccount bankAccount, CancellationToken cancellationToken = default)
	{
		_context.BankAccounts.Add(bankAccount);
		await _context.SaveChangesAsync(cancellationToken);
		return bankAccount;
	}

	public async Task UpdateAsync(BankAccount bankAccount, CancellationToken cancellationToken = default)
	{
		_context.BankAccounts.Update(bankAccount);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateRangeAsync(IEnumerable<BankAccount> bankAccounts, CancellationToken cancellationToken = default)
	{
		_context.BankAccounts.UpdateRange(bankAccounts);
		await _context.SaveChangesAsync(cancellationToken);
	}
}