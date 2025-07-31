namespace BankOfMyHouse.API.Endpoints.Accounts.Create;


public sealed record CreateBankAccountRequestDto
{
	public string Description { get; set; } = string.Empty;
}