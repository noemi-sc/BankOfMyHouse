using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;
using BankOfMyHouse.Domain.Iban.Interfaces;
using BankOfMyHouse.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace BankOfMyHouse.Application.Services.Accounts
{
	public class BankAccountService : IBankAccountService
	{
		private readonly IUserRepository _userRepository;
		private readonly IBankAccountRepository _bankAccountRepository;
		private readonly ITransactionRepository _transactionRepository;
		private readonly ICurrencyRepository _currencyRepository;
		private readonly IPaymentCategoryRepository _paymentCategoryRepository;
		private readonly IIbanGenerator _ibanGenerator;
		private readonly ILogger<BankAccountService> _logger;

		public BankAccountService(
			IUserRepository userRepository,
			IBankAccountRepository bankAccountRepository,
			ITransactionRepository transactionRepository,
			ICurrencyRepository currencyRepository,
			IPaymentCategoryRepository paymentCategoryRepository,
			IIbanGenerator ibanGenerator,
			ILogger<BankAccountService> logger)
		{
			_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
			_bankAccountRepository = bankAccountRepository ?? throw new ArgumentNullException(nameof(bankAccountRepository));
			_transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
			_currencyRepository = currencyRepository ?? throw new ArgumentNullException(nameof(currencyRepository));
			_paymentCategoryRepository = paymentCategoryRepository ?? throw new ArgumentNullException(nameof(paymentCategoryRepository));
			_ibanGenerator = ibanGenerator ?? throw new ArgumentNullException(nameof(ibanGenerator));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Transaction> CreateTransaction(IbanCode sender, IbanCode receiver, decimal amount, string currencyCode, CancellationToken ct, PaymentCategoryCode? paymentCategoryCode = null, string? description = null)
		{
			var categoryCode = paymentCategoryCode ?? PaymentCategoryCode.Other;
			var paymentCategory = await _paymentCategoryRepository.GetByCodeAsync(categoryCode, ct);

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

			var senderAccount = await _bankAccountRepository.GetByIbanWithUserAsync(sender, ct);

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

			var receiverAccount = await _bankAccountRepository.GetByIbanWithUserAsync(receiver, ct);

			var currency = await _currencyRepository.GetByCodeAsync(currencyCode, ct);

			if (currency == null)
			{
				_logger.LogError("Currency with code {CurrencyCode} not found.", currencyCode);
				throw new InvalidOperationException($"Currency with code {currencyCode} not found.");
			}

			senderAccount.Balance -= amount;

			if (receiverAccount != null)
			{
				receiverAccount.Balance += amount;
				await _bankAccountRepository.UpdateRangeAsync([senderAccount, receiverAccount], ct);
			}
			else
			{
				// External transfer - just update sender
				await _bankAccountRepository.UpdateAsync(senderAccount, ct);
				_logger.LogInformation("External transfer to IBAN {Iban} - amount sent out of system.", receiver);
			}

			var transaction = receiverAccount != null
			? Transaction.Create(amount, currency, senderAccount, receiverAccount, paymentCategory, description)
			: Transaction.CreateExternal(amount, currency, senderAccount, receiver, paymentCategory, description);

			await _transactionRepository.AddAsync(transaction, ct);

			return transaction;
		}

		public async Task<BankAccount> GenerateBankAccount(int userId, string? description, CancellationToken cancellationToken)
		{
			var iban = this._ibanGenerator.GenerateItalianIban();

			var ibanExistsAlready = await _bankAccountRepository.IbanExistsAsync(iban, cancellationToken);

			if (ibanExistsAlready)
			{
				_logger.LogWarning("IBAN {Iban} already exists. Generating a new one for user {UserId}.", iban, userId);
				return await GenerateBankAccount(userId, description, cancellationToken); // Recursive call to generate a new IBAN
			}

			var newBankAccount = BankAccount.CreateNew(userId, iban, description);

			return await _bankAccountRepository.AddAsync(newBankAccount, cancellationToken);			
		}

		public async Task<IEnumerable<BankAccount>> GetBankAccounts(int id, CancellationToken ct)
		{
			return await _bankAccountRepository.GetByUserIdAsync(id, ct);
		}

		public async Task<IEnumerable<Transaction>> GetTransactions(int userId, string iban, CancellationToken ct, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
		{
			var user = await _userRepository.GetWithBankAccountsAsync(userId, ct);

			if (user == null)
			{
				return null;
			}

			if (user.BankAccounts.Count(ba => ba.IBAN.Value == iban) == 0)
			{
				_logger.LogWarning("User {UserId} does not have access to IBAN {Iban}", user.Id, iban);
				return null;
			}

			var ibanCode = IbanCode.Create(iban);
			return await _transactionRepository.GetBySenderIbanAsync(ibanCode, startDate, endDate, ct);
		}
	}
}
