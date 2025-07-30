using BankOfMyHouse.Domain.BankAccounts;

namespace BankOfMyHouse.Application.Services.Accounts.Interfaces
{
	public interface IBankAccountService
	{
		Task<BankAccount> GenerateBankAccount(int userId);
	}
}
