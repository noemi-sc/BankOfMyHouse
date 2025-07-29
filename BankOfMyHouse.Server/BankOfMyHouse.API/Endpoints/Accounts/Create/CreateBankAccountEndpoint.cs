using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Server.Endpoints.Accounts.Create;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankOfMyHouse.API.Endpoints.Accounts.Create;

public class CreateBankAccountEndpoint : Endpoint<CreateBankAccountRequestDto,
                                                  Results<Ok<CreateBankAccountResponseDto>,
                                                  NotFound,
                                                  ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/bankAccount");

        //AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        //Description(b => b.Produces(403));
        Tags(nameof(BankAccount));
    }

    public override Task HandleAsync(CreateBankAccountRequestDto req, CancellationToken ct)
    {
        var x = User.Identities;

        return base.HandleAsync(req, ct);
    }
}
