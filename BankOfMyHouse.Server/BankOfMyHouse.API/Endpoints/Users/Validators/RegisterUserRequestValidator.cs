using BankOfMyHouse.API.Endpoints.Users.Register;
using FastEndpoints;
using FluentValidation;

namespace BankOfMyHouse.API.Endpoints.Users.Validators;

public class RegisterUserRequestValidator : Validator<RegisterUserRequestDto>
{
	public RegisterUserRequestValidator()
	{
		RuleFor(x => x.Username)
			.NotEmpty()
			.WithMessage("Username is required")
			.Length(3, 50)
			.WithMessage("Username must be between 3 and 50 characters")
			.Matches("^[a-zA-Z0-9_-]+$")
			.WithMessage("Username can only contain letters, numbers, underscores, and hyphens");

		RuleFor(x => x.Email)
			.NotEmpty()
			.WithMessage("Email is required")
			.EmailAddress()
			.WithMessage("Invalid email format")
			.MaximumLength(255)
			.WithMessage("Email cannot exceed 255 characters");

		RuleFor(x => x.Password)
			.NotEmpty()
			.WithMessage("Password is required")
			.MinimumLength(8)
			.WithMessage("Password must be at least 8 characters long")
			.Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
			.WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character");

		RuleFor(x => x.ConfirmPassword)
			.NotEmpty()
			.WithMessage("Password confirmation is required")
			.Equal(x => x.Password)
			.WithMessage("Passwords do not match");

		RuleFor(x => x.FirstName)
			.MaximumLength(50)
			.WithMessage("First name cannot exceed 50 characters")
			.When(x => !string.IsNullOrEmpty(x.FirstName));

		RuleFor(x => x.LastName)
			.MaximumLength(50)
			.WithMessage("Last name cannot exceed 50 characters")
			.When(x => !string.IsNullOrEmpty(x.LastName));
	}
}
