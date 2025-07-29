namespace BankOfMyHouse.API.Endpoints.Users.RefreshToken;

public sealed record RefreshTokenRequestDto
{
	public string AccessToken { get; set; } = string.Empty;
	public string RefreshToken { get; set; } = string.Empty;
}
