using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;
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

		public async Task<Transaction> CreateTransaction(IbanCode sender, IbanCode receiver, decimal amount, CancellationToken ct)
		{
			if (sender.Value == receiver.Value)
			{
				_logger.LogError("Sender and receiver IBANs are the same: {Iban}", sender);
				throw new InvalidOperationException("Sender and receiver IBANs cannot be the same.");
			}

			var senderAccount = await this._dbContext.BankAccounts
				.Include(x => x.User)
				.SingleOrDefaultAsync(x => x.IBAN.Value == sender.Value, ct);

			if (senderAccount == null)
			{
				_logger.LogError("Sender account with IBAN {Iban} not found.", sender);
				throw new InvalidOperationException($"Sender account with IBAN {sender} not found.");
			}

			if (senderAccount.Balance < amount)
			{
				_logger.LogError("Insufficient funds in sender account with IBAN {Iban}.", sender);
				throw new InvalidOperationException($"Insufficient funds in sender account with IBAN {sender}.");
			}

			var receiverAccount = await this._dbContext.BankAccounts
				.Include(x => x.User)
				.SingleOrDefaultAsync(x => x.IBAN.Value == receiver.Value, ct);

			if (receiverAccount == null)
			{
				_logger.LogError("Receiver account with IBAN {Iban} not found.", receiver);
				throw new InvalidOperationException($"Receiver account with IBAN {receiver} not found.");
			}

			senderAccount.Balance -= amount;
			receiverAccount.Balance += amount;

			this._dbContext.BankAccounts.UpdateRange([senderAccount, receiverAccount]);

			var transaction = Transaction.CreateNew(amount, senderAccount, receiverAccount, PaymentCategory.Other);

			await this._dbContext.Transactions.AddAsync(transaction, ct);

			await this._dbContext.SaveChangesAsync(ct);

			return transaction;
		}

		public async Task<BankAccount> GenerateBankAccount(int userId)
		{
			var iban = this._ibanGenerator.GenerateItalianIban();

			var ibanExistsAlready = await this._dbContext.BankAccounts
				.AnyAsync(x => x.IBAN.Value == iban.Value);

			if (ibanExistsAlready)
			{
				_logger.LogWarning("IBAN {Iban} already exists. Generating a new one for user {UserId}.", iban, userId);
				return await GenerateBankAccount(userId); // Recursive call to generate a new IBAN
			}

			var newBankAccount = BankAccount.CreateNew(userId, iban);

			await this._dbContext.BankAccounts.AddAsync(newBankAccount);

			await this._dbContext.SaveChangesAsync();

			return newBankAccount;
		}

		public async Task<ICollection<BankAccount>> GetBankAccounts(int id, CancellationToken ct)
		{
			return await this._dbContext.BankAccounts.Where(x => x.UserId == id)
				.ToListAsync(ct);
		}
	}
}
