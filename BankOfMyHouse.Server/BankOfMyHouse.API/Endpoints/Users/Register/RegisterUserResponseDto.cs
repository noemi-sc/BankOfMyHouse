using BankOfMyHouse.API.Endpoints.Users.DTOs;

namespace BankOfMyHouse.API.Endpoints.Users.Register
{
	public sealed record RegisterUserResponseDto
	{
		public string Message { get; set; } = string.Empty;
		public UserDto User { get; set; } = new();
		public string? AccessToken { get; set; } // Optional: auto-login after registration
		public string? RefreshToken { get; set; }
	}
}
