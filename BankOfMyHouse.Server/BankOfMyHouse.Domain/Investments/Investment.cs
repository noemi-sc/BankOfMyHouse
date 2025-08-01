using BankOfMyHouse.Domain.BankAccounts;

namespace BankOfMyHouse.Domain.Investments
{
	public class Investment
	{
		private Investment()
		{
		}

		public Investment(decimal sharesAmount, int companyId, int bankAccountId)
		{
			SharesAmount = sharesAmount;
			CompanyId = companyId;
			BankAccountId = bankAccountId;
			CreatedAt = DateTimeOffset.UtcNow;
		}

		public int Id { get; init; }
		public decimal SharesAmount { get; init; }
		public DateTimeOffset CreatedAt { get; init; }
		public int CompanyId { get; init; }
		public int BankAccountId { get; init; }

		//NAVIGATIONS
		public Company Company { get; init; }
		public BankAccount BankAccount { get; init; }
	}
}
