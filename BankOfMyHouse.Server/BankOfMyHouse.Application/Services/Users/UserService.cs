using BankOfMyHouse.Application.Services.Users.Interfaces;
using BankOfMyHouse.Domain.Users;
using BankOfMyHouse.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankOfMyHouse.Application.Services.Users;

public class UserService : IUserService
{
	private readonly BankOfMyHouseDbContext _context;
	private readonly ILogger<UserService> _logger;
	private readonly PasswordHasher<User> _passwordHasher;

	public UserService(BankOfMyHouseDbContext context, ILogger<UserService> logger)
	{
		_context = context;
		_logger = logger;
		_passwordHasher = new PasswordHasher<User>();
	}

	public async Task<User?> ValidateCredentialsAsync(string username, string password)
	{
		try
		{
			var user = await _context.Users
				.Include(u => u.UserRoles)
				.ThenInclude(u => u.Role)
				.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

			if (user == null)
			{
				_logger.LogWarning("User not found or inactive: {Username}", username);
				return null;
			}

			var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
			if (result == PasswordVerificationResult.Failed)
			{
				_logger.LogWarning("Invalid password for user: {Username}", username);
				return null;
			}

			if (result == PasswordVerificationResult.SuccessRehashNeeded)
			{
				user.PasswordHash = _passwordHasher.HashPassword(user, password);
			}

			user.LastLoginAt = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			return user;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error validating credentials for user: {Username}", username);
			return null;
		}
	}

	public async Task<User?> GetUserByUsernameAsync(string username)
	{
		return await _context.Users
			.Include(u => u.UserRoles)
			.FirstOrDefaultAsync(u => u.Username == username);
	}

	public async Task<User?> GetUserByEmailAsync(string email)
	{
		return await _context.Users
			.Include(u => u.UserRoles)
			.FirstOrDefaultAsync(u => u.Email == email);
	}

	public async Task<User?> GetUserWithRolesAsync(int userId)
	{
		return await _context.Users
			.Include(u => u.UserRoles)
			.ThenInclude(u => u.Role)
			.FirstOrDefaultAsync(u => u.Id == userId);
	}

	private async Task<bool> IsUsernameAvailableAsync(string username)
	{
		return !await _context.Users.AnyAsync(u => u.Username == username);
	}

	private async Task<bool> IsEmailAvailableAsync(string email)
	{
		return !await _context.Users.AnyAsync(u => u.Email == email);
	}

	public async Task<User> RegisterUserAsync(User userToRegister, string password)
	{
		using var transaction = await _context.Database.BeginTransactionAsync();

		try
		{
			// Check if username is available
			if (!await IsUsernameAvailableAsync(userToRegister.Username))
			{
				throw new InvalidOperationException("Username is already taken");
			}

			// Check if email is available
			if (!await IsEmailAvailableAsync(userToRegister.Email))
			{
				throw new InvalidOperationException("Email is already registered");
			}

			// Create new user
			var user = new User(
				userToRegister.Username,
				userToRegister.Email,
				password,
				userToRegister.FirstName,
				userToRegister.LastName
			);

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			// Assign default role
			await AssignDefaultRoleAsync(user);

			await transaction.CommitAsync();

			_logger.LogInformation("User {Username} registered successfully", user.Username);

			return user;
		}
		catch (Exception ex)
		{
			await transaction.RollbackAsync();
			_logger.LogError(ex, "Error registering user: {Username}", userToRegister.Username);
			throw;
		}
	}

	public async Task<bool> AssignDefaultRoleAsync(User user)
	{
		try
		{
			var defaultRole = await _context.Roles
				.FirstOrDefaultAsync(r => r.Name == "BankUser");

			if (defaultRole == null)
			{
				_logger.LogWarning("Default 'User' role not found");
				return false;
			}

			var userRole = new UserRole
			{
				UserId = user.Id,
				RoleId = defaultRole.Id,
				AssignedAt = DateTimeOffset.UtcNow
			};

			_context.UserRoles.Add(userRole);
			await _context.SaveChangesAsync();

			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error assigning default role to user: {UserId}", user.Id);
			return false;
		}
	}
}