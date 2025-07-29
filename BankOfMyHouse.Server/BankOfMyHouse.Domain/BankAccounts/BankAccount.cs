using BankOfMyHouse.Domain.Iban;
using BankOfMyHouse.Domain.Iban.Interfaces;

namespace BankOfMyHouse.Domain.BankAccounts;

public class BankAccount
{
	// Parameterless constructor for EF Core
	private BankAccount() { }

	private BankAccount(int userId, IbanCode iban)
	{
		Id = Guid.NewGuid();
		UserId = userId;
		IBAN = iban;
		CreationDate = DateTimeOffset.Now;
	}

	public Guid Id { get; init; }
	public IbanCode IBAN { get; init; }
	public int UserId { get; init; }
	public DateTimeOffset CreationDate { get; init; }

	// Navigation property
	public User User { get; init; } // Add this property
	public List<Transaction> Transactions { get; init; } // Add this property	

	public static BankAccount CreateNew(int userId, IIbanGenerator ibanGenerator)
	{
		var iban = ibanGenerator.GenerateItalianIban();
		return new BankAccount(userId, iban);
	}

	// Factory method for reconstruction from persistence
	public static BankAccount Reconstitute(int userId, IbanCode iban)
	{
		return new BankAccount(userId, iban);
	}
}
