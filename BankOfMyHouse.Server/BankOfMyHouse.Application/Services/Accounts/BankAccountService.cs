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

		public async Task<Transaction> CreateTransaction(IbanCode sender, IbanCode receiver, decimal amount, string currencyCode, CancellationToken ct, PaymentCategoryCode? paymentCategoryCode = null, string? description = null)
		{
			var categoryCode = paymentCategoryCode ?? PaymentCategoryCode.Other;
			var paymentCategory = await _dbContext.PaymentCategories
				.FirstOrDefaultAsync(pc => pc.Code == categoryCode, ct);
			
			if (paymentCategory == null)
			{
				_logger.LogError("PaymentCategory '{CategoryCode}' not found in database.", categoryCode);
				throw new InvalidOperationException($"PaymentCategory '{categoryCode}' not found in database.");
			}
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

			var currency = await this._dbContext.Currencies
				.SingleOrDefaultAsync(c => c.Code == currencyCode, ct);

			if (currency == null)
			{
				_logger.LogError("Currency with code {CurrencyCode} not found.", currencyCode);
				throw new InvalidOperationException($"Currency with code {currencyCode} not found.");
			}

			senderAccount.Balance -= amount;

			if (receiverAccount != null)
			{
				receiverAccount.Balance += amount;
				this._dbContext.BankAccounts.UpdateRange([senderAccount, receiverAccount]);
			}
			else
			{
				// External transfer - just update sender
				this._dbContext.BankAccounts.Update(senderAccount);
				_logger.LogInformation("External transfer to IBAN {Iban} - amount sent out of system.", receiver);
			}

			var transaction = receiverAccount != null 
			? Transaction.Create(amount, currency, senderAccount, receiverAccount, paymentCategory, description)
			: Transaction.CreateExternal(amount, currency, senderAccount, receiver, paymentCategory, description);

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

		public async Task<IEnumerable<Transaction>> GetTransactions(int userId, string iban, CancellationToken ct, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
		{
			var user = await this._dbContext.Users.Include(u => u.BankAccounts).FirstOrDefaultAsync(x => x.Id == userId, ct);

			if (user == null)
			{	
				return null;
			}

			if (user.BankAccounts.Count(ba => ba.IBAN.Value == iban) == 0)
			{
				_logger.LogWarning("User {UserId} does not have access to IBAN {Iban}", user.Id, iban);
				return null;
			}

			var query = this._dbContext.Transactions
				.Include(t => t.Currency)
				.Include(t => t.PaymentCategory)
				.Where(x => x.Sender.Value == iban);

			if (startDate.HasValue)
			{
				query = query.Where(x => x.TransactionCreation >= startDate.Value);
			}
			if (endDate.HasValue)
			{
				query = query.Where(x => x.TransactionCreation <= endDate.Value);
			}

			return await query.ToListAsync(ct);
		}
	}
}
