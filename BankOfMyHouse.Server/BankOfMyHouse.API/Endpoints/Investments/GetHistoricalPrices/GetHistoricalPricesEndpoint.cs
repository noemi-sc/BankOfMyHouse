using BankOfMyHouse.Application.Services.Investments.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Investments.GetHistoricalPrices
{
	public class GetHistoricalPricesEndpoint : Endpoint<GetHistoricalPricesRequestDto, GetHistoricalPricesResponseDto>
	{
		private readonly IInvestmentService _investmentService;
		private readonly IUserService _userService;
		private readonly ILogger<GetHistoricalPricesEndpoint> _logger;

		public GetHistoricalPricesEndpoint(
			IInvestmentService investmentService,
			IUserService userService,
			ILogger<GetHistoricalPricesEndpoint> logger)
		{
			this._investmentService = investmentService ?? throw new ArgumentNullException(nameof(investmentService));
			this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override void Configure()
		{
			Get("/investments/historical-prices");
			Roles("BankUser");
			Summary(s =>
			{
				s.Summary = "Get Historical Stock Prices";
				s.Description = "Get historical stock prices for all companies within a specified time range";
				s.Responses[200] = "Historical prices retrieved successfully";
				s.Responses[400] = "Invalid request parameters";
				s.Responses[500] = "Internal server error";
			});
		}

		public override async Task HandleAsync(GetHistoricalPricesRequestDto req, CancellationToken ct)
		{
			var userIdClaim = User.FindFirst("userId")?.Value;
			if (!int.TryParse(userIdClaim, out int userId))
			{
				_logger.LogError("Invalid user ID claim: {UserIdClaim}", userIdClaim);
				await Send.ErrorsAsync(400, ct);
				return;
			}

			var user = await _userService.GetUserWithRolesAsync(userId, ct);
			if (user == null)
			{
				await Send.NotFoundAsync(ct);
				return;
			}

			DateTime startDate;
			if (req.StartDate.HasValue)
			{
				startDate = req.StartDate.Value.ToUniversalTime();
			}
			else
			{
				startDate = DateTime.UtcNow.AddHours(-req.Hours);
			}

			var historicalPrices = await this._investmentService.GetHistoricalStockPrices(startDate, req.CompanyId, ct);

			var response = new GetHistoricalPricesResponseDto
			{
				CompanyPrices = historicalPrices.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Select(sp => new CompanyStockPriceDto
					{
						StockPrice = sp.StockPrice,
						TimeOfPriceChange = sp.TimeOfPriceChange,
						CompanyId = sp.CompanyId
					}).ToList()
				)
			};

			await Send.OkAsync(response, ct);
		}
	}
}
