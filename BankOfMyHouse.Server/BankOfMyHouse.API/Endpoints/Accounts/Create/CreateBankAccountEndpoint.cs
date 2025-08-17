using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Accounts.Create;

public class CreateBankAccountEndpoint : Endpoint<CreateBankAccountRequestDto, CreateBankAccountResponseDto>
{
	private readonly IBankAccountService _bankAccountService;
	private readonly IUserService _userService;
	private readonly ILogger<CreateBankAccountEndpoint> _logger;

	public CreateBankAccountEndpoint(
		IBankAccountService bankAccountService,
		IUserService userService,
		ILogger<CreateBankAccountEndpoint> logger)
	{
		this._bankAccountService = bankAccountService ?? throw new ArgumentNullException(nameof(bankAccountService));
		this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
		this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public override void Configure()
	{
		Post("/bankAccounts");
		Roles("BankUser");
		Summary(s =>
		{
			s.Summary = "Create Bank Account";
			s.Description = "Create a new bank account for the authenticated user";
			s.Responses[201] = "Bank account created successfully";
			s.Responses[400] = "Invalid request or user already has account";
			s.Responses[404] = "User not found";
			s.Responses[500] = "Internal server error";
		});
	}

	public override async Task HandleAsync(CreateBankAccountRequestDto req, CancellationToken ct)
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

		var newBankAccountForUser = await this._bankAccountService.GenerateBankAccount(user.Id);

		var response = new CreateBankAccountResponseDto
		{
			IBAN = newBankAccountForUser.IBAN.Value,
		};

		await Send.CreatedAtAsync<CreateBankAccountEndpoint>(responseBody: response, cancellation: ct);
	}
}
