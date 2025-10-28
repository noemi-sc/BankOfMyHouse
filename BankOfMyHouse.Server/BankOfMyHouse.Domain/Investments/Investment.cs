using BankOfMyHouse.Domain.BankAccounts;

namespace BankOfMyHouse.Domain.Investments
{
	public class Investment
	{
		private Investment()
		{
		}

		public Investment(decimal sharesAmount, int companyId, int bankAccountId, decimal purchasePrice)
		{
			SharesAmount = sharesAmount;
			CompanyId = companyId;
			BankAccountId = bankAccountId;
			PurchasePrice = purchasePrice;
			CreatedAt = DateTimeOffset.UtcNow;
		}

		public int Id { get; init; }
		public decimal SharesAmount { get; init; }
		public decimal PurchasePrice { get; init; }
		public DateTimeOffset CreatedAt { get; init; }
		public int CompanyId { get; init; }
		public int BankAccountId { get; init; }

		//NAVIGATIONS
		public required Company Company { get; init; }
		public required BankAccount BankAccount { get; init; }

		public static Investment Create(decimal sharesAmount, Company company, BankAccount bankAccount, decimal purchasePrice)
		{
			return new Investment(sharesAmount, company.Id, bankAccount.Id, purchasePrice)
			{
				Company = company,
				BankAccount = bankAccount
			};
		}
	}
}
