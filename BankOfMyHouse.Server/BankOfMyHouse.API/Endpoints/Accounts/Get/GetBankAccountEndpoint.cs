using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;

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
            s.Summary = "Get BankAccounts for the Current User";
            s.Description = "Get BankAccount for the Current User"; ;
            s.Responses[200] = "List of BankAccounts of the user";
            s.Responses[404] = "BankAccount for the user not found";
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
            BankAccounts = bankAccounts.Select(ba => new
            {
                ba.IBAN,
                ba.Balance
            }).ToList()
        };

        await Send.CreatedAtAsync<GetBankAccountEndpoint>(
            routeValues: new { },
            responseBody: response,
            cancellation: ct);
    }
}
