using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Accounts.Create;


public class CreateBankAccountRequestDto
{
    [FromBody]
    public required string Name { get; set; }
}