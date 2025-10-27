using BankOfMyHouse.Application.Services.Investments.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;
using Mapster;

namespace BankOfMyHouse.API.Endpoints.Investments.Get
{
	public class GetInvestmentsEndpoint : Endpoint<GetInvestmentRequestDto, GetInvestmentsResponseDto>
	{
		private readonly IInvestmentService _investmentService;
		private readonly IUserService _userService;
		private readonly ILogger<GetInvestmentsEndpoint> _logger;

		public GetInvestmentsEndpoint(
			IInvestmentService investmentService,
			IUserService userService,
			ILogger<GetInvestmentsEndpoint> logger)
		{
			this._investmentService = investmentService ?? throw new ArgumentNullException(nameof(investmentService));
			this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override void Configure()
		{
			Get("/investments");
			Roles("BankUser");
			Summary(s =>
			{
				s.Summary = "Get Investment";
				s.Description = "Get investment for the authenticated user";
				s.Responses[200] = "List of investments for the user";
				s.Responses[404] = "No investment found for the user";
				s.Responses[500] = "Internal server error";
			});
		}

		public override async Task HandleAsync(GetInvestmentRequestDto req, CancellationToken ct)
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

			var investments = await this._investmentService.GetInvestments(user.Id, ct);

			var response = new GetInvestmentsResponseDto
			{
				Investments = investments.Adapt<List<InvestmentDto>>()
			};

			await Send.CreatedAtAsync<GetInvestmentsEndpoint>(responseBody: response, cancellation: ct);
		}
	}
}
