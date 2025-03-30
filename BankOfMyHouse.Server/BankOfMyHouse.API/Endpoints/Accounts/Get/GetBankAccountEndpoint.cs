using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Server.Endpoints.Accounts.Get;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankOfMyHouse.Server.Endpoints.Accounts;

public class GetBankAccountEndpoint : Endpoint<GetBankAccountRequest,
                                                  Results<Ok<GetBankAccountResponse>,
                                                  NotFound,
                                                  ProblemDetails>>
{
    public override void Configure()
    {
        Get("/bankAccount");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(b => b.Produces(403));
        Tags(nameof(BankAccount));
    }

    public override Task HandleAsync(GetBankAccountRequest req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}
