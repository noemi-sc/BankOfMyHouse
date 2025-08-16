using BankOfMyHouse.Domain.Iban;

namespace BankOfMyHouse.Domain.BankAccounts;

public sealed record Transaction
{
	//EF CORE
	private Transaction() { }

	private Transaction(decimal amount, IbanCode sender, IbanCode receiver, PaymentCategory category = PaymentCategory.Other, string? description = null)
	{
		Id = Guid.NewGuid();
		Amount = amount;
		TransactionCreation = DateTimeOffset.UtcNow;
		Sender = sender;
		Receiver = receiver;
		PaymentCategory = category;
		Currency = new Currency("Euro", "EUR");
		Description = description;
	}

	public Guid Id { get; set; }
	public decimal Amount { get; set; }
	public DateTimeOffset TransactionCreation { get; set; }
	public PaymentCategory PaymentCategory { get; set; }
	public Currency Currency { get; set; }
	public string? Description { get; set; }

	// NAVIGATION PROPERTIES
	public IbanCode Sender { get; set; }
	public IbanCode Receiver { get; set; }

	public static Transaction CreateNew(decimal amount, BankAccount senderAccount, BankAccount receiverAccount, PaymentCategory paymentCategory, string? description)
	{
		return new Transaction(amount, senderAccount.IBAN, receiverAccount.IBAN, paymentCategory, description);
	}
}
