using BankOfMyHouse.API.Endpoints.Users.DTOs;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using FastEndpoints;

namespace BankOfMyHouse.API.Endpoints.Users.Login
{
	public class LoginEndpoint : Endpoint<LoginRequestDto, LoginResponseDto>
	{
		private readonly IUserService _userService;
		private readonly IJwtService _jwtService;
		private readonly ILogger<LoginEndpoint> _logger;

		public LoginEndpoint(
			IUserService userService,
			IJwtService jwtService,
			ILogger<LoginEndpoint> logger)
		{
			_userService = userService;
			_jwtService = jwtService;
			_logger = logger;
		}

		public override void Configure()
		{
			Post("/users/auth/login");
			AllowAnonymous();
			Validator<LoginRequestValidator>();
			Summary(s =>
			{
				s.Summary = "User Login";
				s.Description = "Authenticate user with username and password";
				s.Responses[200] = "Login successful";
				s.Responses[400] = "Invalid input";
				s.Responses[401] = "Invalid credentials";
			});
		}

		public override async Task HandleAsync(LoginRequestDto req, CancellationToken ct)
		{
			try
			{
				var user = await _userService.ValidateCredentialsAsync(req.Username, req.Password);
				if (user == null)
				{
					await Send.UnauthorizedAsync(ct);
					return;
				}

				var accessToken = _jwtService.GenerateAccessToken(user);
				var refreshToken = _jwtService.GenerateRefreshToken(user);

				var response = new LoginResponseDto
				{
					AccessToken = accessToken,
					RefreshToken = refreshToken,
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

				_logger.LogInformation("User {Username} logged in successfully", user.Username);

				await Send.OkAsync(response, ct);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login for user: {Username}", req.Username);

				await Send.ErrorsAsync(500, ct);
			}
		}
	}
}