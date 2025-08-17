using BankOfMyHouse.Domain.Iban;
using BankOfMyHouse.Domain.Investments;
using BankOfMyHouse.Domain.Users;

namespace BankOfMyHouse.Domain.BankAccounts;

public class BankAccount
{
	// Parameterless constructor for EF Core
	private BankAccount() { }

	private BankAccount(int? userId, IbanCode iban)
	{
		UserId = userId;
		IBAN = iban;
		CreationDate = DateTimeOffset.UtcNow;		
	}

	public int Id { get; set; }
	public IbanCode IBAN { get; init; }
	public int? UserId { get; init; }
	public DateTimeOffset CreationDate { get; init; }
	public decimal Balance { get; set; } = 0;

	// Navigation property
	public User? User { get; set; }
	public ICollection<Investment> Investments { get; set; }

	public static BankAccount CreateNew(int userId, IbanCode ibanCode)
	{
		return new BankAccount(userId, ibanCode);
	}

	// Factory method for external bank accounts (from other banks)
	public static BankAccount CreateExternal(IbanCode ibanCode)
	{
		return new BankAccount(null, ibanCode);
	}

	// Factory method for reconstruction from persistence
	public static BankAccount Reconstitute(int? userId, IbanCode iban)
	{
		return new BankAccount(userId, iban);
	}
}
