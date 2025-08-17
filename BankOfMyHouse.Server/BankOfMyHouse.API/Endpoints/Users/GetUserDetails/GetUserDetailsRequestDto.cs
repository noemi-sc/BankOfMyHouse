using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Users.GetUserDetails;

public sealed record GetUserDetailsRequestDto
{
	[FromClaim("userId")]
	public int UserId { get; set; }
}