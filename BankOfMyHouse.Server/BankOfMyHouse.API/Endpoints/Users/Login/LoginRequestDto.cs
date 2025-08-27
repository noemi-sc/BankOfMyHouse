namespace BankOfMyHouse.API.Endpoints.Users.Login;

public sealed record LoginRequestDto
{
	public required string Username { get; set; } = string.Empty;
	public required string Password { get; set; } = string.Empty;
}
