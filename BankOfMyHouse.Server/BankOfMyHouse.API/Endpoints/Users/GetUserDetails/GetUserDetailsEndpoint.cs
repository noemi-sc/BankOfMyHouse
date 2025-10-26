using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Users.GetUserDetails;

public class GetUserDetailsEndpoint : Endpoint<GetUserDetailsRequestDto, GetUserDetailsResponseDto>
{
	private readonly IUserService _userService;
	private readonly IBankAccountService _bankAccountService;
	private readonly ILogger<GetUserDetailsEndpoint> _logger;

	public GetUserDetailsEndpoint(
		IUserService userService,
		IBankAccountService bankAccountService,
		ILogger<GetUserDetailsEndpoint> logger)
	{
		_userService = userService ?? throw new ArgumentNullException(nameof(userService));
		_bankAccountService = bankAccountService ?? throw new ArgumentNullException(nameof(bankAccountService));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public override void Configure()
	{
		Get("/users/details");
		Roles("BankUser");
		Summary(s =>
		{
			s.Summary = "Get User Details";
			s.Description = "Retrieve detailed information about the authenticated user including bank accounts and balances";
			s.Responses[200] = "User details retrieved successfully";
			s.Responses[404] = "User not found";
			s.Responses[500] = "Internal server error";
		});
	}

	public override async Task HandleAsync(GetUserDetailsRequestDto req, CancellationToken ct)
	{
		var user = await _userService.GetUserWithRolesAsync(req.UserId, ct);
		if (user == null)
		{
			await Send.NotFoundAsync(ct);
			return;
		}

		var bankAccounts = await _bankAccountService.GetBankAccounts(req.UserId, ct);

		var response = new GetUserDetailsResponseDto
		{
			Id = user.Id,
			Username = user.Username,
			Email = user.Email,
			FirstName = user.FirstName,
			LastName = user.LastName,
			CreatedAt = user.CreatedAt,
			LastLoginAt = user.LastLoginAt,
			IsActive = user.IsActive,
			Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
			BankAccounts = bankAccounts.Select(ba => new BankAccountDetailsDto
			{
				Id = ba.Id,
				IBAN = ba.IBAN.Value,
				Balance = ba.Balance,
				CreationDate = ba.CreationDate,
				Description = ba.Description
			}).ToList()
		};

		await Send.OkAsync(response, ct);
	}
}