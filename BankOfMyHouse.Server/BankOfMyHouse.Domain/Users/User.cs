using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Users;
using Microsoft.AspNetCore.Identity;

public class User
{
	private static readonly PasswordHasher<User> _passwordHasher = new();

	public int Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
	public string? FirstName { get; set; } // Optional
	public string? LastName { get; set; } // Optional
	public DateTime CreatedAt { get; set; }
	public DateTime? LastLoginAt { get; set; }
	public bool IsActive { get; set; } = true;
	public bool EmailConfirmed { get; set; } = false; // For email confirmation feature

	// Navigation properties
	public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
	public ICollection<Role> Roles { get; set; } = new List<Role>();
	public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

	// Existing constructors...
	public User()
	{
		CreatedAt = DateTime.UtcNow;
		IsActive = true;
		UserRoles = new List<UserRole>();
		Roles = new List<Role>();
	}

	public User(string username, string email, string password, string? firstName = null, string? lastName = null) : this()
	{
		Username = username ?? throw new ArgumentNullException(nameof(username));
		Email = email ?? throw new ArgumentNullException(nameof(email));
		FirstName = firstName;
		LastName = lastName;

		if (string.IsNullOrWhiteSpace(password))
			throw new ArgumentException("Password cannot be null or empty", nameof(password));

		PasswordHash = _passwordHasher.HashPassword(this, password);
	}

	// Helper property for full name
	public string FullName => $"{FirstName} {LastName}".Trim();

	// Existing methods...
	public bool VerifyPassword(string password)
	{
		if (string.IsNullOrWhiteSpace(password))
			return false;

		var result = _passwordHasher.VerifyHashedPassword(this, PasswordHash, password);
		return result == PasswordVerificationResult.Success ||
			   result == PasswordVerificationResult.SuccessRehashNeeded;
	}

	public void UpdatePassword(string newPassword)
	{
		if (string.IsNullOrWhiteSpace(newPassword))
			throw new ArgumentException("Password cannot be null or empty", nameof(newPassword));

		PasswordHash = _passwordHasher.HashPassword(this, newPassword);
	}

	public void UpdateLastLogin()
	{
		LastLoginAt = DateTime.UtcNow;
	}

	public void Activate()
	{
		IsActive = true;
	}

	public void Deactivate()
	{
		IsActive = false;
	}

	public void ConfirmEmail()
	{
		EmailConfirmed = true;
	}
}