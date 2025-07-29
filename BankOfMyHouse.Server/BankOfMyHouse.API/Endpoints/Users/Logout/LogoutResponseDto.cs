namespace BankOfMyHouse.API.Endpoints.Users.Logout
{
	public sealed record LogoutResponseDto
	{
		public string Message { get; set; } = string.Empty;
	}
}
