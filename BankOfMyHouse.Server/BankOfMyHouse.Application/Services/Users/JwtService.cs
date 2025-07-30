using BankOfMyHouse.Application.Configurations;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using BankOfMyHouse.Domain.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankOfMyHouse.Application.Services.Users;

public class JwtService : IJwtService
{

	private readonly JwtSettings _jwtSettings;
	private readonly ILogger<JwtService> _logger;

	public JwtService(IOptions<JwtSettings> jwtSettings, ILogger<JwtService> logger)
	{
		_jwtSettings = jwtSettings.Value;
		_logger = logger;
	}

	public string GenerateAccessToken(User user)
	{
		return GenerateToken(user, _jwtSettings.ExpirationInMinutes);
	}

	public string GenerateRefreshToken(User user)
	{
		return GenerateToken(user, _jwtSettings.RefreshTokenExpirationInDays * 24 * 60); // Convert days to minutes
	}

	private string GenerateToken(User user, int expirationInMinutes)
	{
		var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim("userId", user.Id.ToString()),
				new Claim("username", user.Username),
				new Claim("tokenType", expirationInMinutes > 120 ? "refresh" : "access") // Simple way to identify token type
            }),
			Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
			Issuer = _jwtSettings.Issuer,
			Audience = _jwtSettings.Audience,
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		// Add role claims
		foreach (var userRole in user.UserRoles)
		{
			tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, userRole.Role.Name));
		}

		var tokenHandler = new JwtSecurityTokenHandler();
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public ClaimsPrincipal? GetPrincipalFromToken(string token)
	{
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateAudience = true,
			ValidAudience = _jwtSettings.Audience,
			ValidateIssuer = true,
			ValidIssuer = _jwtSettings.Issuer,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
			ValidateLifetime = false // Don't validate lifetime when extracting claims for refresh
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		try
		{
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

			if (securityToken is not JwtSecurityToken jwtSecurityToken ||
				!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error validating token");
			return null;
		}
	}

	public bool ValidateToken(string token)
	{
		try
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
				ValidateIssuer = true,
				ValidIssuer = _jwtSettings.Issuer,
				ValidateAudience = true,
				ValidAudience = _jwtSettings.Audience,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};

			tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
			return true;
		}
		catch
		{
			return false;
		}
	}
}