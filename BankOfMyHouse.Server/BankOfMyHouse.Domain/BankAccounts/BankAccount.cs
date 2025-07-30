using BankOfMyHouse.Domain.Iban;
using BankOfMyHouse.Domain.Iban.Interfaces;
using BankOfMyHouse.Domain.Investments;
using BankOfMyHouse.Domain.Users;

namespace BankOfMyHouse.Domain.BankAccounts;

public class BankAccount
{
	// Parameterless constructor for EF Core
	private BankAccount() { }

	private BankAccount(int userId, IbanCode iban)
	{
		UserId = userId;
		IBAN = iban;
		CreationDate = DateTimeOffset.Now;
	}

	public int Id { get; set; }
	public IbanCode IBAN { get; init; }
	public int UserId { get; init; }
	public DateTimeOffset CreationDate { get; init; }

	// Navigation property
	public User User { get; set; }
	public ICollection<Transaction> Transactions { get; set; }
	public ICollection<Investment> Investments { get; set; }

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
