using BankOfMyHouse.API.Endpoints.Users.DTOs;
using BankOfMyHouse.Application.Users.Interfaces;
using BankOfMyHouse.Domain.Users;
using FastEndpoints;
using IMapper = MapsterMapper.IMapper;

namespace BankOfMyHouse.API.Endpoints.Users.Register;

public class RegisterUserEndpoint : Endpoint<RegisterUserRequestDto, RegisterUserResponseDto>
{
	private readonly IUserService _userService;
	private readonly IJwtService _jwtService;
	private readonly ILogger<RegisterUserEndpoint> _logger;
	private readonly IMapper _mapper;

	public RegisterUserEndpoint(
		IUserService userService,
		IJwtService jwtService,
		ILogger<RegisterUserEndpoint> logger,
		IMapper mapper)
	{
		this._userService = userService;
		this._jwtService = jwtService;
		this._logger = logger;
		this._mapper = mapper;
	}

	public override void Configure()
	{
		Post("/users/auth/register");
		AllowAnonymous();
		Summary(s =>
		{
			s.Summary = "Register New User";
			s.Description = "Register a new user account";
			s.Responses[201] = "User registered successfully";
			s.Responses[400] = "Invalid input or user already exists";
			s.Responses[500] = "Internal server error";
		});
	}

	public override async Task HandleAsync(RegisterUserRequestDto req, CancellationToken ct)
	{
		try
		{
			// Register the user
			var user = await _userService.RegisterUserAsync(this._mapper.Map<User>(req), req.Password);

			// Load user with roles for token generation
			var userWithRoles = await _userService.GetUserWithRolesAsync(user.Id);
			if (userWithRoles == null)
			{
				AddError("Failed to retrieve user after registration");
				await Send.ErrorsAsync(500, ct);
				return;
			}

			// Optionally auto-login user after registration
			var accessToken = _jwtService.GenerateAccessToken(userWithRoles);
			var refreshToken = _jwtService.GenerateRefreshToken(userWithRoles);

			var response = new RegisterUserResponseDto
			{
				Message = "User registered successfully",
				User = new UserDto
				{
					Id = userWithRoles.Id,
					Username = userWithRoles.Username,
					Email = userWithRoles.Email,
					CreatedAt = userWithRoles.CreatedAt,
					LastLoginAt = userWithRoles.LastLoginAt,
					IsActive = userWithRoles.IsActive,
					Roles = userWithRoles.UserRoles.Select(r => r.Role.Name).ToList()
				},
				AccessToken = accessToken,
				RefreshToken = refreshToken
			};

			_logger.LogInformation("User {Username} registered and logged in successfully", user.Username);

			await Send.CreatedAtAsync<GetCurrentUserEndpoint>(
				routeValues: new { },
				responseBody: response,
				cancellation: ct);
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogWarning(ex, "Registration failed for user: {Username}", req.Username);
			AddError(ex.Message);
			await Send.ErrorsAsync(400, ct);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error during registration for user: {Username}", req.Username);
			AddError("An error occurred during registration. Please try again.");
			await Send.ErrorsAsync(500, ct);
		}
	}
}
