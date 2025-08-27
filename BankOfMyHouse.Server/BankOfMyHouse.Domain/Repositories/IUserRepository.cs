using BankOfMyHouse.Domain.Users;

namespace BankOfMyHouse.Domain.Repositories;

public interface IUserRepository
{
	Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<User?> GetWithRolesAsync(int id, CancellationToken cancellationToken = default);
	Task<User?> GetWithBankAccountsAsync(int id, CancellationToken cancellationToken = default);
	Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
	Task UpdateAsync(User user, CancellationToken cancellationToken = default);
	Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken = default);
}