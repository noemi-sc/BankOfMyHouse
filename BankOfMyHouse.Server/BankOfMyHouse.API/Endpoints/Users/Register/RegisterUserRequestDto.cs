namespace BankOfMyHouse.API.Endpoints.Users.Register;

public sealed record RegisterUserRequestDto
{
	public string Username { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string ConfirmPassword { get; set; } = string.Empty;
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
}
