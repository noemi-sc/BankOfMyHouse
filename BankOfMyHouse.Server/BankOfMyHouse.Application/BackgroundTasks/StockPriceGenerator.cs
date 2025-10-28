using BankOfMyHouse.Application.DTOs;
using BankOfMyHouse.Application.Hubs;
using BankOfMyHouse.Domain.Investments;
using BankOfMyHouse.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankOfMyHouse.Application.BackgroundTasks;

public class StockPriceGenerator : BackgroundService
{
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly ILogger<StockPriceGenerator> _logger;
	private readonly Random _random = new Random();
	private readonly IHubContext<InvestmentHub> _hubContext;

	public StockPriceGenerator(IServiceScopeFactory serviceScopeFactory, ILogger<StockPriceGenerator> logger, IHubContext<InvestmentHub> hubContext)
	{
		_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Stock Price Generator started");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await UpdateStockPrices(stoppingToken);
				await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
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
				await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
			}
		}

		_logger.LogInformation("Stock Price Generator stopped");
	}

	private async Task UpdateStockPrices(CancellationToken cancellationToken)
	{
		using var scope = _serviceScopeFactory.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<BankOfMyHouseDbContext>();

		var companies = await dbContext.Companies.ToListAsync(cancellationToken);

		if (companies.Count <= 0)
		{
			_logger.LogWarning("No companies found to update stock prices");
			return;
		}

		// Fetch all latest prices in ONE query for efficiency
		var latestPrices = await dbContext.CompanyStockPrices
			.GroupBy(sp => sp.CompanyId)
			.Select(g => new
			{
				CompanyId = g.Key,
				LatestPrice = g.OrderByDescending(sp => sp.TimeOfPriceChange).First()
			})
			.ToListAsync(cancellationToken);

		var latestPriceDict = latestPrices.ToDictionary(lp => lp.CompanyId, lp => lp.LatestPrice.StockPrice);

		var newPrices = new List<CompanyStockPrice>();

		foreach (var company in companies)
		{
			decimal oldPrice = 0.0m;

			// Get the latest price from dictionary
			if (latestPriceDict.TryGetValue(company.Id, out var lastPrice))
			{
				oldPrice = lastPrice;
			}
			else
			{
				// No price history exists, generate initial price
				oldPrice = Random.Shared.Next(1000, 200001) / 1000.0m;
			}

			var percentageChange = (decimal)(_random.NextDouble() * 20 - 10); // Random between -10 and +10
			var newPrice = oldPrice * (1 + percentageChange / 100);

			// Ensure price stays within reasonable bounds
			if (newPrice < 1) newPrice = 1;
			if (newPrice > 1000) newPrice = 1000;

			var companyPrice = new CompanyStockPrice(newPrice, company.Id);
			newPrices.Add(companyPrice);

			_logger.LogDebug("Updated {CompanyName} stock price from {OldPrice:C} to {NewPrice:C} ({Change:+0.00;-0.00}%)",
				company.Name, oldPrice, newPrice, percentageChange);
		}

		// Add all prices at once
		await dbContext.CompanyStockPrices.AddRangeAsync(newPrices, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		
		var pricesDictionary = newPrices.ToDictionary(
			cp => cp.CompanyId,
			cp => new StockPriceDto
			{
				StockPrice = cp.StockPrice,
				TimeOfPriceChange = cp.TimeOfPriceChange,
				CompanyId = cp.CompanyId
			}
		);

		await _hubContext.Clients.All.SendAsync("TransferAllPrices", pricesDictionary, cancellationToken);

		_logger.LogInformation("Updated and broadcast stock prices for {CompanyCount} companies", companies.Count);
	}
}