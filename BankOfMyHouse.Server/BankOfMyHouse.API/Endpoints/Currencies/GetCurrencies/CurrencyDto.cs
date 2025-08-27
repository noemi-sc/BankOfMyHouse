namespace BankOfMyHouse.API.Endpoints.Currencies.GetCurrencies;

public sealed record CurrencyDto
{
	public int Id { get; set; }
	public string Code { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Symbol { get; set; } = string.Empty;
}