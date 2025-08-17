using BankOfMyHouse.API.Endpoints.Transactions.DTOs;
using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using FastEndpoints;
using Mapster;

namespace BankOfMyHouse.API.Endpoints.Transactions.GetTransactions;

public class GetTransactionsEndpoint : Endpoint<GetTransactionsRequestDto, GetTransactionsResponseDto>
{
	private readonly IBankAccountService _bankAccountService;
	private readonly ILogger<GetTransactionsEndpoint> _logger;

	public GetTransactionsEndpoint(
		IBankAccountService bankAccountService,
		ILogger<GetTransactionsEndpoint> logger)
	{
		_bankAccountService = bankAccountService ?? throw new ArgumentNullException(nameof(bankAccountService));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public override void Configure()
	{
		Get("/transactions");
		Roles("BankUser");
		Summary(s =>
		{
			s.Summary = "Get Transactions for Current User";
			s.Description = "Retrieve all transactions for the authenticated user's bank accounts";
			s.Responses[200] = "List of transactions for the user";
			s.Responses[404] = "No transactions found for the user";
			s.Responses[500] = "Internal server error";
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

		var transactions = await _bankAccountService.GetTransactions(userId, req.Iban, ct, req.StartDate, req.EndDate);

		var response = new GetTransactionsResponseDto
		{
			Transactions = transactions?.Adapt<List<TransactionDto>>() ?? new List<TransactionDto>()
		};

		await Send.OkAsync(response, ct);
	}
}
