using BankOfMyHouse.Domain.Iban;

namespace BankOfMyHouse.Domain.BankAccounts;

public sealed record Transaction
{
	//EF CORE
	private Transaction() 
	{
		PaymentCategory = null!;
		Currency = null!;
		Sender = null!;
		Receiver = null!;
	}


	public Guid Id { get; set; }
	public decimal Amount { get; set; }
	public DateTimeOffset TransactionCreation { get; set; }
	public int PaymentCategoryId { get; set; }
	public required PaymentCategory PaymentCategory { get; set; }
	public int CurrencyId { get; set; }
	public required Currency Currency { get; set; }
	public string? Description { get; set; }

	// NAVIGATION PROPERTIES
	public required IbanCode Sender { get; set; }
	public required IbanCode Receiver { get; set; }

	public static Transaction Create(decimal amount, Currency currency, BankAccount senderAccount, BankAccount receiverAccount, PaymentCategory paymentCategory, string? description)
	{
		return new Transaction
		{
			Id = Guid.NewGuid(),
			Amount = amount,
			TransactionCreation = DateTimeOffset.UtcNow,
			PaymentCategoryId = paymentCategory.Id,
			CurrencyId = currency.Id,
			Description = description,
			PaymentCategory = paymentCategory,
			Currency = currency,
			Sender = senderAccount.IBAN,
			Receiver = receiverAccount.IBAN
		};
	}

	public static Transaction CreateExternal(decimal amount, Currency currency, BankAccount senderAccount, IbanCode receiverIban, PaymentCategory paymentCategory, string? description)
	{
		return new Transaction
		{
			Id = Guid.NewGuid(),
			Amount = amount,
			TransactionCreation = DateTimeOffset.UtcNow,
			PaymentCategoryId = paymentCategory.Id,
			CurrencyId = currency.Id,
			Description = description,
			PaymentCategory = paymentCategory,
			Currency = currency,
			Sender = senderAccount.IBAN,
			Receiver = receiverIban
		};
	}
}
