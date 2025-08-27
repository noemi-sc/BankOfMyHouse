using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Accounts.Create;


public sealed record CreateBankAccountRequestDto
{
	[FromClaim("userId")]
	public required int UserId { get; set; }
	public string Description { get; set; } = string.Empty;
}