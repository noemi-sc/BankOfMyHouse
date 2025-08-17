namespace BankOfMyHouse.API.Endpoints.Accounts.Create;

public sealed record CreateBankAccountResponseDto
{
	public string IBAN { get; set; } = string.Empty;
}