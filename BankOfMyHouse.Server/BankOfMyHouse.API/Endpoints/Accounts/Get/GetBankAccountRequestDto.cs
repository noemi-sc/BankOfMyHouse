using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Accounts.Get;


public class GetBankAccountRequestDto
{
    [FromBody]
    public required string UserId { get; set; }
}