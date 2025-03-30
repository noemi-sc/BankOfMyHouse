using FastEndpoints;

namespace BankOfMyHouse.Server.Endpoints.Accounts.Create;


public class CreateBankAccountRequest
{
    [FromBody]
    public required string Name { get; set; }
}