using BankOfMyHouse.Domain.Investments;
using BankOfMyHouse.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class StockPriceGenerator : BackgroundService
{
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly ILogger<StockPriceGenerator> _logger;
	private readonly Random _random = new Random();

	public StockPriceGenerator(IServiceScopeFactory serviceScopeFactory, ILogger<StockPriceGenerator> logger)
	{
		_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Stock Price Generator started");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await UpdateStockPrices(stoppingToken);
				await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
			}
			catch (OperationCanceledException)
			{
				// Expected when cancellation is requested
				break;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating stock prices");
				// Continue running even if there's an error
				await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
			}
		}

		_logger.LogInformation("Stock Price Generator stopped");
	}

	private async Task UpdateStockPrices(CancellationToken cancellationToken)
	{
		using var scope = _serviceScopeFactory.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<BankOfMyHouseDbContext>();

		var companies = await dbContext.Companies
			.Include(c => c.StockPriceHistory.OrderByDescending(h => h.TimeOfPriceChange).Take(1))
			.ToListAsync(cancellationToken);

		if (companies.Count <= 0)
		{
			_logger.LogWarning("No companies found to update stock prices");
			return;
		}

		foreach (var company in companies)
		{
			decimal oldPrice = 0.0m;

			var percentageChange = (decimal)(_random.NextDouble() * 20 - 10); // Random between -10 and +10

			if (company.StockPriceHistory.Count <= 0)
			{
				oldPrice = Random.Shared.Next(1000, 200001) / 1000.0m;
			}
			else
			{
				oldPrice = company.StockPriceHistory.First().StockPrice;
			}

			var newPrice = oldPrice * (1 + percentageChange / 100);

			await dbContext.AddAsync(new CompanyStockPrice(newPrice, company.Id));

			_logger.LogDebug("Updated {CompanyName} stock price from {OldPrice:C} to {NewPrice:C} ({Change:+0.00;-0.00}%)",
				company.Name, oldPrice, newPrice, percentageChange);
		}

		await dbContext.SaveChangesAsync(cancellationToken);
		_logger.LogInformation("Updated stock prices for {CompanyCount} companies", companies.Count);
	}
}