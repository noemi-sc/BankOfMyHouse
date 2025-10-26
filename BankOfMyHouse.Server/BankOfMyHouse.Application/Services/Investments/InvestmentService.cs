using BankOfMyHouse.Application.Services.Investments.Interfaces;
using BankOfMyHouse.Domain.Investments;
using BankOfMyHouse.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankOfMyHouse.Application.Services.Investments
{
	public class InvestmentService : IInvestmentService
	{
		private readonly BankOfMyHouseDbContext _context;
		private readonly ILogger<InvestmentService> _logger;

		public InvestmentService(BankOfMyHouseDbContext context, ILogger<InvestmentService> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Investment> CreateInvestment(int userId, int bankAccountId, int companyId, decimal investmentAmount, CancellationToken ct)
		{
			var user = await this._context.Users
				.Include(x => x.BankAccounts)
				.SingleOrDefaultAsync(x => x.Id == userId, ct);

			if (user is null || !user.BankAccounts.Any(x => x.Id == bankAccountId))
			{
				throw new InvalidOperationException("User or bankAccount for the user not found");
			}

			var bankAccount = user.BankAccounts.First(x => x.Id == bankAccountId);

			this._context.BankAccounts.Update(bankAccount);
			this._context.SaveChanges();

			if (bankAccount.Balance < investmentAmount)
			{
				throw new InvalidOperationException("Insufficient funds in the bank account.");
			}

			bankAccount.Balance -= investmentAmount;

			this._context.Users.Update(user);

			var company = await this._context.Companies
				.Include(c => c.StockPriceHistory.OrderByDescending(h => h.TimeOfPriceChange)
				.Take(1))
				.SingleOrDefaultAsync(x => x.Id == companyId, ct);

			if (company is null)
			{
				throw new InvalidOperationException("Company not found.");
			}

			var currentStockPrice = company.StockPriceHistory.First();

			var sharesAmount = Math.Round(investmentAmount / currentStockPrice.StockPrice * 1000 / 1000, 3);

			var investment = Investment.Create(sharesAmount, company, bankAccount);

			await this._context.Investments.AddAsync(investment);

			await this._context.SaveChangesAsync();

			return investment;
		}

		public async Task<List<Company>> GetCompanies(CancellationToken ct)
		{
			return await this._context.Companies.ToListAsync(ct);
		}

		public Task<Investment> WithdrawInvestment(int userId, int companyId, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public async Task<List<Investment>> GetInvestments(int userId, CancellationToken ct)
		{
			return await this._context.Investments
				.Include(i => i.Company)
				.Include(i => i.BankAccount)
				.Where(i => i.BankAccount.UserId == userId)
				.ToListAsync(ct);
		}
	}
}
