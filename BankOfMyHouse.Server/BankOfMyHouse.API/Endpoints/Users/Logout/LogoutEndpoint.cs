using BankOfMyHouse.API.Endpoints.Users.Logout;
using FastEndpoints;

public class LogoutEndpoint : EndpointWithoutRequest<LogoutResponseDto>
{
	public override void Configure()
	{
		Post("/users/auth/logout");
		Roles("User", "Admin", "Moderator", "Manager");
		Summary(s =>
		{
			s.Summary = "User Logout";
			s.Description = "Logout current user";
			s.Responses[200] = "Logout successful";
		});
	}

	public override async Task HandleAsync(CancellationToken ct)
	{
		// With stateless tokens, logout is mainly a client-side operation
		var response = new LogoutResponseDto
		{
			Message = "Logged out successfully. Please delete tokens from client storage."
		};

		await Send.OkAsync(response, ct);
	}
}