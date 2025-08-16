using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using BankOfMyHouse.Domain.BankAccounts;
using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Transactions.GetTransactions;

public class GetTransactionsEndpoint : Endpoint<GetTransactionsRequestDto, GetTransactionsResponseDto>
{
	private readonly IBankAccountService _bankAccountService;
	private readonly IUserService _userService;
	private readonly ILogger<GetTransactionsEndpoint> _logger;

	public GetTransactionsEndpoint(
		IBankAccountService bankAccountService,
		IUserService userService,
		ILogger<GetTransactionsEndpoint> logger)
	{
		_bankAccountService = bankAccountService ?? throw new ArgumentNullException(nameof(bankAccountService));
		_userService = userService ?? throw new ArgumentNullException(nameof(userService));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public override void Configure()
	{
		Get("/transactions");
		Roles("BankUser");
		Summary(s =>
		{
			s.Summary = $"Get {nameof(Transaction)}s for the Current ";
			s.Description = "Get {nameof(Transaction)}s for the Current User"; ;
			s.Responses[200] = $"List of {nameof(Transaction)} of the user";
			s.Responses[404] = $"{nameof(Transaction)}s for the user not found";
			s.Responses[500] = $"Internal server error";
		});
	}

	public override async Task HandleAsync(GetTransactionsRequestDto req, CancellationToken ct)
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

		var transactions = await _bankAccountService.GetTransactions(user, req.Iban, ct, req.StartDate, req.EndDate);

		var response = new GetTransactionsResponseDto
		{
			Transactions = transactions
		};

		await Send.OkAsync(response, ct);
	}
}
