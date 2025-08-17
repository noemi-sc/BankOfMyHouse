using FastEndpoints;
using FluentValidation;

namespace BankOfMyHouse.API.Endpoints.Users.Register;

public class RegisterUserRequestValidator : Validator<RegisterUserRequestDto>
{
	public RegisterUserRequestValidator()
	{
		RuleFor(x => x.Username)
			.NotEmpty()
			.WithMessage("Username is required")
			.MinimumLength(3)
			.WithMessage("Username must be at least 3 characters")
			.MaximumLength(50)
			.WithMessage("Username cannot exceed 50 characters")
			.Matches("^[a-zA-Z0-9_-]+$")
			.WithMessage("Username can only contain letters, numbers, underscores, and hyphens");

		RuleFor(x => x.Email)
			.NotEmpty()
			.WithMessage("Email is required")
			.EmailAddress()
			.WithMessage("Email must be a valid email address")
			.MaximumLength(255)
			.WithMessage("Email cannot exceed 255 characters");

		RuleFor(x => x.Password)
			.NotEmpty()
			.WithMessage("Password is required")
			.MinimumLength(8)
			.WithMessage("Password must be at least 8 characters")
			.MaximumLength(100)
			.WithMessage("Password cannot exceed 100 characters")
			.Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
			.WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character");

		RuleFor(x => x.ConfirmPassword)
			.NotEmpty()
			.WithMessage("Confirm password is required")
			.Equal(x => x.Password)
			.WithMessage("Passwords do not match");

		RuleFor(x => x.FirstName)
			.MaximumLength(100)
			.WithMessage("First name cannot exceed 100 characters")
			.When(x => !string.IsNullOrEmpty(x.FirstName));

		RuleFor(x => x.LastName)
			.MaximumLength(100)
			.WithMessage("Last name cannot exceed 100 characters")
			.When(x => !string.IsNullOrEmpty(x.LastName));
	}
}