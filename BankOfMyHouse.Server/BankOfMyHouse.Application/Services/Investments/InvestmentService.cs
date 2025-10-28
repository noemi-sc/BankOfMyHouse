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

			if (bankAccount.Balance < investmentAmount)
			{
				throw new InvalidOperationException("Insufficient funds in the bank account.");
			}

			bankAccount.Balance -= investmentAmount;

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

			_logger.LogInformation("Creating investment - PurchasePrice: {PurchasePrice}, SharesAmount: {SharesAmount}, InvestmentAmount: {InvestmentAmount}",
				currentStockPrice.StockPrice, sharesAmount, investmentAmount);

			var investment = Investment.Create(sharesAmount, company, bankAccount, currentStockPrice.StockPrice);

			_logger.LogInformation("Investment object created - PurchasePrice: {PurchasePrice}, Id: {Id}", investment.PurchasePrice, investment.Id);

			try
			{
				this._context.Users.Update(user);
				this._context.BankAccounts.Update(bankAccount);
				await this._context.Investments.AddAsync(investment, ct);
				await this._context.SaveChangesAsync(ct);
			}
			catch (Exception ex)
			{
				_logger.LogError("Error occured while creating a new investment {Exception}", ex);
				throw new InvalidOperationException("Error occured while creating a new investment", ex);
			}
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

		public async Task<Dictionary<int, List<CompanyStockPrice>>> GetHistoricalStockPrices(DateTime startDate, int? companyId, CancellationToken ct)
		{
			// Fetch stock prices directly with optimized query
			var query = this._context.CompanyStockPrices
				.Where(sp => sp.TimeOfPriceChange >= startDate);

			// Filter by company if provided
			if (companyId.HasValue)
			{
				query = query.Where(sp => sp.CompanyId == companyId.Value);
			}

			var stockPrices = await query
				.OrderBy(sp => sp.CompanyId)
				.ThenBy(sp => sp.TimeOfPriceChange)
				.ToListAsync(ct);

			// Group by company ID in memory (more efficient than grouping in SQL)
			var result = stockPrices
				.GroupBy(sp => sp.CompanyId)
				.ToDictionary(
					group => group.Key,
					group => group.ToList()
				);

			return result;
		}
	}
}
