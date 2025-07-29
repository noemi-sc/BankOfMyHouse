using BankOfMyHouse.API.Endpoints.Users.Login;
using FastEndpoints;
using FluentValidation;

namespace BankOfMyHouse.API.Endpoints.Users.Validators;

public class LoginRequestValidator : Validator<LoginRequestDto>
{
	public LoginRequestValidator()
	{
		RuleFor(x => x.Username)
			.NotEmpty()
			.WithMessage("Username is required")
			.Length(3, 50)
			.WithMessage("Username must be between 3 and 50 characters");

		RuleFor(x => x.Password)
			.NotEmpty()
			.WithMessage("Password is required")
			.MinimumLength(6)
			.WithMessage("Password must be at least 6 characters long");
	}
}
