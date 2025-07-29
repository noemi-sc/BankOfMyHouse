using FastEndpoints;

namespace BankOfMyHouse.Server.Endpoints.Accounts.Create;


public class CreateBankAccountRequestDto
{
    [FromBody]
    public required string Name { get; set; }
}