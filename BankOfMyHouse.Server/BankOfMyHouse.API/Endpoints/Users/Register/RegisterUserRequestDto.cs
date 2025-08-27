namespace BankOfMyHouse.API.Endpoints.Users.Register;

public sealed record RegisterUserRequestDto
{
	public required string Username { get; set; } = string.Empty;
	public required string Email { get; set; } = string.Empty;
	public required string Password { get; set; } = string.Empty;
	public required string ConfirmPassword { get; set; } = string.Empty;
	public required string? FirstName { get; set; }
	public required string? LastName { get; set; }
}
