using BankOfMyHouse.Domain.Iban;
using BankOfMyHouse.Domain.Iban.Interfaces;
using BankOfMyHouse.Domain.Users;

namespace BankOfMyHouse.Domain.BankAccounts;

public class BankAccount
{
    public Guid Id { get; init; }
    public IbanCode IBAN { get; init; }
    public string UserId { get; init; }
    public DateTimeOffset CreationDate { get; init; }

    // Navigation property
    public ApplicationUser User { get; init; } // Add this property
    public List<Transaction> Transactions { get; init; } // Add this property

    // Parameterless constructor for EF Core
    private BankAccount() { }

    private BankAccount(string userId, IbanCode iban)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        IBAN = iban;
        CreationDate = DateTimeOffset.Now;
    }

    public static BankAccount CreateNew(string userId, IIbanGenerator ibanGenerator)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        var iban = ibanGenerator.GenerateItalianIban();
        return new BankAccount(userId, iban);
    }

    // Factory method for reconstruction from persistence
    public static BankAccount Reconstitute(string userId, IbanCode iban)
    {
        return new BankAccount(userId, iban);
    }
}
