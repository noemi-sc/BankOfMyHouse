using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Server.Endpoints.Accounts;
using BankOfMyHouse.Server.Endpoints.Accounts.Get;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankOfMyHouse.API.Endpoints.Accounts.Get;

public class GetBankAccountEndpoint : Endpoint<GetBankAccountRequestDto,
                                                  Results<Ok<GetBankAccountResponseDto>,
                                                  NotFound,
                                                  ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/bankAccount");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(b => b.Produces(403));
        Tags(nameof(BankAccount));
    }

    public override Task HandleAsync(GetBankAccountRequestDto req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}
