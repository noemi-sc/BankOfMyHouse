using BankOfMyHouse.Domain.Iban;

namespace BankOfMyHouse.Domain.BankAccounts
{
	public class Transaction
	{
		public Guid Id { get; set; }
		public float Amount { get; set; }
		public DateTimeOffset TransactionCreation { get; set; }
		public PaymentCategory PaymentCategory { get; set; }

		public IbanCode	Sender { get; set; }
		public IbanCode	Receiver { get; set; }
	}
}
