namespace BankOfMyHouse.API.Endpoints.Users.GetUserDetails;

public sealed record GetUserDetailsResponseDto
{
	public int Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? LastLoginAt { get; set; }
	public bool IsActive { get; set; }
	public List<string> Roles { get; set; } = new();
	public List<BankAccountDetailsDto> BankAccounts { get; set; } = new();
}

public sealed record BankAccountDetailsDto
{
	public int Id { get; set; }
	public string IBAN { get; set; } = string.Empty;
	public decimal Balance { get; set; }
	public DateTimeOffset CreationDate { get; set; }
}