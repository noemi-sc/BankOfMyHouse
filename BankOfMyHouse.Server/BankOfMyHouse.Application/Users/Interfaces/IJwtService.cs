using BankOfMyHouse.Domain.Users;
using System.Security.Claims;

namespace BankOfMyHouse.Application.Users.Interfaces
{
	public interface IJwtService
	{
		string GenerateAccessToken(User user);
		string GenerateRefreshToken(User user);
		ClaimsPrincipal? GetPrincipalFromToken(string token);
		bool ValidateToken(string token);
	}
}
