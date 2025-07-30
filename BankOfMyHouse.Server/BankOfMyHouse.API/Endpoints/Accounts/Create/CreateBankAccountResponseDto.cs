using BankOfMyHouse.Domain.Iban;

namespace BankOfMyHouse.API.Endpoints.Accounts.Create;

public sealed record CreateBankAccountResponseDto
{
	public IbanCode	IBAN { get; set; }
}