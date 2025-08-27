namespace BankOfMyHouse.API.Endpoints.Accounts.Get;

public sealed record BankAccountDto
{
	public int Id { get; set; }
	public string IBAN { get; set; } = string.Empty;
	public decimal Balance { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
}