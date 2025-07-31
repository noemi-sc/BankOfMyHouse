using BankOfMyHouse.API.Endpoints.Users.DTOs;
using BankOfMyHouse.API.Endpoints.Users.RefreshToken;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;

public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequestDto, RefreshTokenResponseDto>
{
	private readonly IUserService _userService;
	private readonly IJwtService _jwtService;
	private readonly ILogger<RefreshTokenEndpoint> _logger;

	public RefreshTokenEndpoint(
		IUserService userService,
		IJwtService jwtService,
		ILogger<RefreshTokenEndpoint> logger)
	{
		_userService = userService;
		_jwtService = jwtService; 
		_logger = logger;
	}

	public override void Configure()
	{
		Post("/users/auth/refresh");
		AllowAnonymous();
		Summary(s =>
		{
			s.Summary = "Refresh Access Token";
			s.Description = "Get new access token using refresh token";
			s.Responses[200] = "Token refreshed successfully";
			s.Responses[400] = "Invalid tokens";
		});
	}

	public override async Task HandleAsync(RefreshTokenRequestDto req, CancellationToken ct)
	{
		try
		{
			// Validate the refresh token
			var principal = _jwtService.GetPrincipalFromToken(req.RefreshToken);
			if (principal == null)
			{
				AddError("Invalid refresh token");
				await Send.ErrorsAsync(400, ct);
				return;
			}

			// Check if it's actually a refresh token
			var tokenType = principal.FindFirst("tokenType")?.Value;
			if (tokenType != "refresh")
			{
				AddError("Invalid token type");
				await Send.ErrorsAsync(400, ct);
				return;
			}

			// Check if refresh token is still valid (not expired)
			if (!_jwtService.ValidateToken(req.RefreshToken))
			{
				AddError("Refresh token has expired");
				await Send.ErrorsAsync(400, ct);
				return;
			}

			// Get user ID and fetch user data
			var userIdClaim = principal.FindFirst("userId")?.Value;
			if (!int.TryParse(userIdClaim, out int userId))
			{
				AddError("Invalid user ID in token");
				await Send.ErrorsAsync(400, ct);
				return;
			}

			var user = await _userService.GetUserWithRolesAsync(userId, ct);
			if (user == null || !user.IsActive)
			{
				AddError("User not found or inactive");
				await Send.ErrorsAsync(400, ct);
				return;
			}

			// Generate new tokens
			var newAccessToken = _jwtService.GenerateAccessToken(user);
			var newRefreshToken = _jwtService.GenerateRefreshToken(user);

			var response = new RefreshTokenResponseDto
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken,
				ExpiresAt = DateTime.UtcNow.AddMinutes(60),
				User = new UserDto
				{
					Id = user.Id,
					Username = user.Username,
					Email = user.Email,
					CreatedAt = user.CreatedAt,
					LastLoginAt = user.LastLoginAt,
					IsActive = user.IsActive,
					Roles = user.UserRoles.Select(r => r.Role.Name).ToList()
				}
			};

			await Send.OkAsync(response, ct);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error during token refresh");
			await Send.ErrorsAsync(500, ct);
		}
	}
}