namespace BankOfMyHouse.API.Endpoints.Accounts.Get;

public class GetBankAccountResponseDto
{
	public required IEnumerable<BankAccountDto> BankAccounts { get; set; }
}