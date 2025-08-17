using BankOfMyHouse.API.Endpoints.Users.DTOs;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Users.CurrentUser;

public class GetCurrentUserEndpoint : EndpointWithoutRequest<GetCurrentUserResponseDto>
{
	private readonly IUserService _userService;
	private readonly ILogger<GetCurrentUserEndpoint> _logger;

	public GetCurrentUserEndpoint(
		IUserService userService,
		ILogger<GetCurrentUserEndpoint> logger)
	{
		_userService = userService;
		_logger = logger;
	}

	public override void Configure()
	{
		Get("/users/auth/me");
		Roles("BankUser");
		Summary(s =>
		{
			s.Summary = "Get Current User";
			s.Description = "Get current authenticated user information";
			s.Responses[200] = "User information retrieved";
			s.Responses[404] = "User not found";
		});
	}

	public override async Task HandleAsync(CancellationToken ct)
	{
		try
		{
			var userIdClaim = User.FindFirst("userId")?.Value;
			if (!int.TryParse(userIdClaim, out int userId))
			{
				await Send.ErrorsAsync(400, ct);
				return;
			}

			var user = await _userService.GetUserWithRolesAsync(userId, ct);
			if (user == null)
			{
				await Send.NotFoundAsync(ct);
				return;
			}

			var userDto = new UserDto
			{
				Id = user.Id,
				Username = user.Username,
				Email = user.Email,
				CreatedAt = user.CreatedAt,
				LastLoginAt = user.LastLoginAt,
				IsActive = user.IsActive,
				Roles = user.UserRoles.Select(r => r.Role.Name).ToList()
			};

			var response = new GetCurrentUserResponseDto
			{
				User = userDto
			};

			await Send.OkAsync(response, ct);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting current user");

			await Send.ErrorsAsync(500, ct);
		}
	}
}