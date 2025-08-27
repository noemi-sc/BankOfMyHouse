
using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using BankOfMyHouse.Domain.Iban;
using FastEndpoints;
using Mapster;

namespace BankOfMyHouse.API.Endpoints.Transactions.CreateTransaction;

public class CreateTransactionEndpoint : Endpoint<CreateTransactionRequestDto, CreateTransactionResponseDto>
{
	private readonly IBankAccountService _bankAccountService;
	private readonly IUserService _userService;
	private readonly ILogger<CreateTransactionEndpoint> _logger;

	public CreateTransactionEndpoint(
		IBankAccountService bankAccountService,
		IUserService userService,
		ILogger<CreateTransactionEndpoint> logger)
	{
		this._bankAccountService = bankAccountService ?? throw new ArgumentNullException(nameof(bankAccountService));
		this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
		this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public override void Configure()
	{
		Post("/transactions");
		Roles("BankUser");
		Validator<CreateTransactionRequestValidator>();
		Summary(s =>
		{
			s.Summary = "Create Transaction";
			s.Description = "Create a new transaction from user's bank account to another account";
			s.Responses[201] = "Transaction created successfully";
			s.Responses[400] = "Invalid transaction data or insufficient funds";
			s.Responses[404] = "User or sender account not found";
			s.Responses[500] = "Internal server error";
		});
	}

	public override async Task HandleAsync(CreateTransactionRequestDto req, CancellationToken ct)
	{
		var user = await _userService.GetUserWithRolesAsync(req.UserId, ct);
		if (user == null)
		{
			await Send.NotFoundAsync(ct);
			return;
		}

		var transaction = await this._bankAccountService.CreateTransaction(
			req.SenderIban.Adapt<IbanCode>(),
			req.ReceiverIban.Adapt<IbanCode>(),
			req.Amount,
			req.CurrencyCode,
			ct,
			req.PaymentCategory,
			req.Description);

		var response = new CreateTransactionResponseDto
		{
			TransactionId = transaction.Id,
			CreatedAt = transaction.TransactionCreation
		};

		await Send.CreatedAtAsync<CreateTransactionEndpoint>(responseBody: response, cancellation: ct);
	}
}
