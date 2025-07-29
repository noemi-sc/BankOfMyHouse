using BankOfMyHouse.API.Endpoints.Users.DTOs;

namespace BankOfMyHouse.API.Endpoints.Users.Login;

public sealed record LoginResponseDto
{
	public string AccessToken { get; set; } = string.Empty;
	public string RefreshToken { get; set; } = string.Empty;
	public DateTime ExpiresAt { get; set; }
	public UserDto User { get; set; } = new();
}