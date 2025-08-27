using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;
using Mapster;

namespace BankOfMyHouse.API.Endpoints.Accounts.Get;

public class GetBankAccountEndpoint : EndpointWithoutRequest<GetBankAccountResponseDto>
{
	private readonly IBankAccountService _bankAccountService;
	private readonly IUserService _userService;
	private readonly ILogger<GetBankAccountEndpoint> _logger;

	public GetBankAccountEndpoint(
		IBankAccountService bankAccountService,
		IUserService userService,
		ILogger<GetBankAccountEndpoint> logger)
	{
		this._bankAccountService = bankAccountService ?? throw new ArgumentNullException(nameof(bankAccountService));
		this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
		this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public override void Configure()
	{
		Get("/bankAccounts");
		Roles("BankUser");
		Summary(s =>
		{
			s.Summary = "Get Bank Accounts";
			s.Description = "Retrieve all bank accounts for the authenticated user";
			s.Responses[200] = "Bank accounts retrieved successfully";
			s.Responses[404] = "No bank accounts found for user";
			s.Responses[500] = "Internal server error";
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

		var bankAccounts = await this._bankAccountService.GetBankAccounts(user.Id, ct);

		var response = new GetBankAccountResponseDto
		{
			BankAccounts = bankAccounts.Select(ba => ba.Adapt<BankAccountDto>()).ToList()
		};

		await Send.OkAsync(response, ct);
	}
}
