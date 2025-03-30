using BankOfMyHouse.Domain.BankAccounts;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankOfMyHouse.Server.Endpoints.Accounts.Create;

public class CreateBankAccountEndpoint : Endpoint<CreateBankAccountRequest,
                                                  Results<Ok<CreateBankAccountResponse>,
                                                  NotFound,
                                                  ProblemDetails>>
{
    public override void Configure()
    {
        Post("/bankAccount");

        //AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        //Description(b => b.Produces(403));
        Tags(nameof(BankAccount));
    }

    public override Task HandleAsync(CreateBankAccountRequest req, CancellationToken ct)
    {
        var x = this.User.Identities;

        return base.HandleAsync(req, ct);
    }
}
