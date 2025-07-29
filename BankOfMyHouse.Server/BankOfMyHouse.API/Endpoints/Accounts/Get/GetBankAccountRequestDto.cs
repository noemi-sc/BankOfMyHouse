using FastEndpoints;

namespace BankOfMyHouse.Server.Endpoints.Accounts;


public class GetBankAccountRequestDto
{
    [FromBody]
    public required string UserId { get; set; }
}