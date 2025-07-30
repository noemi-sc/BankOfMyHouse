using BankOfMyHouse.Domain.Users;

namespace BankOfMyHouse.Application.Users.Interfaces;

public interface IUserService
{
	Task<User?> ValidateCredentialsAsync(string username, string password);
	Task<User?> GetUserByUsernameAsync(string username);
	Task<User?> GetUserByEmailAsync(string email);
	Task<User?> GetUserWithRolesAsync(int userId);
	Task<User> RegisterUserAsync(User user, string password);
	Task<bool> AssignDefaultRoleAsync(User user);
}
