using BankOfMyHouse.API.Endpoints.Users.RefreshToken;
using FastEndpoints;
using FluentValidation;

namespace BankOfMyHouse.API.Endpoints.Users.Validators
{
	public class RefreshTokenRequestValidator : Validator<RefreshTokenRequestDto>
	{
		public RefreshTokenRequestValidator()
		{
			RuleFor(x => x.AccessToken)
				.NotEmpty()
				.WithMessage("Access token is required");

			RuleFor(x => x.RefreshToken)
				.NotEmpty()
				.WithMessage("Refresh token is required");
		}
	}
}
