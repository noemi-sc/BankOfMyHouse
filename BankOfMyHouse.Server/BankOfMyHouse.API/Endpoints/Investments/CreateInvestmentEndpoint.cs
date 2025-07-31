using BankOfMyHouse.Application.Services.Investments.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Investments
{
	public class CreateInvestmentEndpoint : Endpoint<CreateInvestmentRequestDto, CreateInvestmentResponseDto>
	{
		private readonly IInvestmentService _investmentService;
		private readonly IUserService _userService;
		private readonly ILogger<CreateInvestmentEndpoint> _logger;

		public CreateInvestmentEndpoint(
			IInvestmentService investmentService,
			IUserService userService,
			ILogger<CreateInvestmentEndpoint> logger)
		{
			this._investmentService = investmentService ?? throw new ArgumentNullException(nameof(investmentService));
			this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override void Configure()
		{
			Post("/investments");
			Roles("BankUser");
			Summary(s =>
			{
				s.Summary = "Create investment for the Current User";
				s.Description = "Create investment for the Current User"; ;
				s.Responses[200] = "investment is created";
				s.Responses[400] = "investment creation has failed";
				s.Responses[500] = "Internal server error";
			});
		}

		public override async Task HandleAsync(CreateInvestmentRequestDto req, CancellationToken ct)
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

			var newInvestment = await this._investmentService.CreateInvestment(user.Id, req.BankAccountId, req.CompanyId, req.InvestmentAmount, ct);
			var response = new CreateInvestmentResponseDto
			{
				SharesAmount = newInvestment.SharesAmount
			};

			await Send.CreatedAtAsync<CreateInvestmentEndpoint>(responseBody: response, cancellation: ct);
		}
	}
}
