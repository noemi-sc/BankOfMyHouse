using BankOfMyHouse.Domain.Iban;
using BankOfMyHouse.Domain.Investments;
using BankOfMyHouse.Domain.Users;

namespace BankOfMyHouse.Domain.BankAccounts;

public class BankAccount
{
	// Parameterless constructor for EF Core
	private BankAccount() { }

	private BankAccount(int? userId, IbanCode iban, string? description)
	{
		UserId = userId;
		IBAN = iban;
		CreationDate = DateTimeOffset.UtcNow;
		Description = description;
	}

	public int Id { get; set; }
	public IbanCode IBAN { get; init; }
	public int? UserId { get; init; }
	public DateTimeOffset CreationDate { get; init; }
	public decimal Balance { get; set; } = 0;
	public string? Description { get; init; }

	// Navigation property
	public User? User { get; set; }
	public ICollection<Investment> Investments { get; set; }

	public static BankAccount CreateNew(int userId, IbanCode ibanCode, string? description)
	{
		return new BankAccount(userId, ibanCode, description);
	}
}
