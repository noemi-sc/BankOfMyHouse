using BankOfMyHouse.Application.Services.Investments.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Investments.ListCompanies
{
	public class ListCompaniesEndpoint : EndpointWithoutRequest<ListCompaniesResponseDto>
	{
		private readonly IInvestmentService _investmentService;
		private readonly IUserService _userService;
		private readonly ILogger<ListCompaniesEndpoint> _logger;

		public ListCompaniesEndpoint(
			IInvestmentService investmentService,
			IUserService userService,
			ILogger<ListCompaniesEndpoint> logger)
		{
			this._investmentService = investmentService ?? throw new ArgumentNullException(nameof(investmentService));
			this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override void Configure()
		{
			Get("/investments/companies");
			Roles("BankUser");
			Summary(s =>
			{
				s.Summary = "List companies";
				s.Description = "List companies";
				s.Responses[200] = "List companies successfully";
			});
		}

		public override async Task HandleAsync(CancellationToken ct)
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

			var companies = await this._investmentService.GetCompanies(ct);

			var response = new ListCompaniesResponseDto
			{
				Companies = companies
			};

			await Send.ResponseAsync(response, cancellation: ct);
		}
	}
}
