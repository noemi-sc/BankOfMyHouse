using BankOfMyHouse.Infrastructure;
using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.API.Endpoints.Currencies.GetCurrencies;

public class GetCurrenciesEndpoint : EndpointWithoutRequest<GetCurrenciesResponseDto>
{
	private readonly BankOfMyHouseDbContext _dbContext;
	private readonly ILogger<GetCurrenciesEndpoint> _logger;

	public GetCurrenciesEndpoint(
		BankOfMyHouseDbContext dbContext,
		ILogger<GetCurrenciesEndpoint> logger)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public override void Configure()
	{
		Get("/currencies");
		Roles("BankUser");
		Summary(s =>
		{
			s.Summary = "Get Currencies";
			s.Description = "Retrieve all available currencies with their symbols, names and codes";
			s.Responses[200] = "Currencies retrieved successfully";
			s.Responses[500] = "Internal server error";
		});
	}

	public override async Task HandleAsync(CancellationToken ct)
	{
		var currencies = await _dbContext.Currencies
			.OrderBy(c => c.Name)
			.ToListAsync(ct);

		var response = new GetCurrenciesResponseDto
		{
			Currencies = currencies.Adapt<List<CurrencyDto>>()
		};

		await Send.OkAsync(response, ct);
	}
}