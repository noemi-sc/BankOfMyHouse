using BankOfMyHouse.API.Endpoints.Users.DTOs;

namespace BankOfMyHouse.API.Endpoints.Users.RefreshToken
{
	public sealed record RefreshTokenResponseDto
	{
		public string AccessToken { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime ExpiresAt { get; set; }
		public UserDto User { get; set; } = new();
	}
}
