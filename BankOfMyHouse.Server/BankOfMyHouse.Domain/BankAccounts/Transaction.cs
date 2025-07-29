using BankOfMyHouse.Domain.Iban;

namespace BankOfMyHouse.Domain.BankAccounts;

public sealed record Transaction
{
	//EF CORE
	public Transaction() { }

	public Transaction(decimal amount, IbanCode sender, IbanCode receiver, PaymentCategory category)
	{
		Id = Guid.NewGuid();
		Amount = amount;
		TransactionCreation = DateTimeOffset.UtcNow;
		Sender = sender;
		Receiver = receiver;
		PaymentCategory = category;
	}

	public Guid Id { get; set; }
	public decimal Amount { get; set; }
	public DateTimeOffset TransactionCreation { get; set; }
	public PaymentCategory PaymentCategory { get; set; }

	// NAVIGATION PROPERTIES
	public IbanCode Sender { get; set; }
	public IbanCode	Receiver { get; set; }
}
