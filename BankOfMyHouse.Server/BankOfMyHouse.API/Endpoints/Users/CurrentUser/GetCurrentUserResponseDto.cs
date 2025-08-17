using BankOfMyHouse.API.Endpoints.Users.DTOs;

namespace BankOfMyHouse.API.Endpoints.Users.CurrentUser;

public sealed record GetCurrentUserResponseDto
{
	public UserDto User { get; set; } = new();
}