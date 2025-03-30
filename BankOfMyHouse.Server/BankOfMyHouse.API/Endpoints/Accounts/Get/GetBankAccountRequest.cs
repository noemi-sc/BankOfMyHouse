using FastEndpoints;

namespace BankOfMyHouse.Server.Endpoints.Accounts;


public class GetBankAccountRequest
{
    [FromBody]
    public required string UserId { get; set; }
}