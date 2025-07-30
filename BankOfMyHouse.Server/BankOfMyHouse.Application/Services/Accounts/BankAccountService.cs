using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban.Interfaces;
using BankOfMyHouse.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankOfMyHouse.Application.Services.Accounts
{
	public class BankAccountService : IBankAccountService
	{
		private readonly BankOfMyHouseDbContext _dbContext;
		private readonly IIbanGenerator _ibanGenerator;
		private readonly ILogger<BankAccountService> _logger;

		public BankAccountService(
			BankOfMyHouseDbContext dbContext,
			IIbanGenerator ibanGenerator,
			ILogger<BankAccountService> logger)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_ibanGenerator = ibanGenerator ?? throw new ArgumentNullException(nameof(ibanGenerator));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<BankAccount> GenerateBankAccount(int userId)
		{
			var iban = this._ibanGenerator.GenerateItalianIban();

			var ibanExistsAlready = await this._dbContext.BankAccounts.AnyAsync(x => x.IBAN == iban);

			if (ibanExistsAlready)
			{
				_logger.LogWarning("IBAN {Iban} already exists. Generating a new one for user {UserId}.", iban, userId);
				return await GenerateBankAccount(userId); // Recursive call to generate a new IBAN
			}

			return BankAccount.CreateNew(userId, iban);
		}
	}
}
