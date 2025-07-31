using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;
using FastEndpoints;
using MapsterMapper;
using IMapper = MapsterMapper.IMapper;

namespace BankOfMyHouse.API.Endpoints.Transactions.CreateTransaction;

public class CreateTransactionEndpoint : Endpoint<CreateTransactionRequestDto, CreateTransactionResponseDto>
{
	private readonly IBankAccountService _bankAccountService;
	private readonly IUserService _userService;
	private readonly IMapper _mapper;
	private readonly ILogger<CreateTransactionEndpoint> _logger;

	public CreateTransactionEndpoint(
		IBankAccountService bankAccountService,
		IUserService userService,
		IMapper mapper,
		ILogger<CreateTransactionEndpoint> logger)
	{
		this._bankAccountService = bankAccountService ?? throw new ArgumentNullException(nameof(bankAccountService));
		this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
		this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public override void Configure()
	{
		Post("/transactions");
		Roles("BankUser");
		Summary(s =>
		{
			s.Summary = $"Create {nameof(Transaction)} for the Current User";
			s.Description = $"Create {nameof(Transaction)} for the Current User"; ;
			s.Responses[200] = $"{nameof(Transaction)} is created";
			s.Responses[400] = $"{nameof(Transaction)} creation has failed";
			s.Responses[500] = "Internal server error";
		});
	}

	public override async Task HandleAsync(CreateTransactionRequestDto req, CancellationToken ct)
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

		var transaction = await this._bankAccountService.CreateTransaction(_mapper.Map<IbanCode>(req.SenderIban), _mapper.Map<IbanCode>(req.ReceiverIban), req.Amount, ct);

		var response = new CreateTransactionResponseDto
		{
			CreatedAt = transaction.TransactionCreation
		};

		await Send.CreatedAtAsync<CreateTransactionEndpoint>(responseBody: response, cancellation: ct);
	}
}
