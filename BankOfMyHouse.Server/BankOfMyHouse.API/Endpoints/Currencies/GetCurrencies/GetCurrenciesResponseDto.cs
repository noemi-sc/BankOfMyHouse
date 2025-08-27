namespace BankOfMyHouse.API.Endpoints.Currencies.GetCurrencies;

public sealed record GetCurrenciesResponseDto
{
	public List<CurrencyDto> Currencies { get; set; } = new();
}
