using BankOfMyHouse.Domain.Repositories;
using BankOfMyHouse.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
	private readonly BankOfMyHouseDbContext _context;

	public UserRepository(BankOfMyHouseDbContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}

	public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		return await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
	}

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
	}

	public async Task<User?> GetWithRolesAsync(int id, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.UserRoles)
			.ThenInclude(ur => ur.Role)
			.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<User?> GetWithBankAccountsAsync(int id, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.BankAccounts)
			.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
	{
		_context.Users.Add(user);
		await _context.SaveChangesAsync(cancellationToken);
		return user;
	}

	public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
	{
		_context.Users.Update(user);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.AnyAsync(u => u.Username == username || u.Email == email, cancellationToken);
	}
}