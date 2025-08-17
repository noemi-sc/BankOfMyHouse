using FastEndpoints;
using FluentValidation;

namespace BankOfMyHouse.API.Endpoints.Investments.Create;

public class CreateInvestmentRequestValidator : Validator<CreateInvestmentRequestDto>
{
	public CreateInvestmentRequestValidator()
	{
		RuleFor(x => x.CompanyId)
			.GreaterThan(0)
			.WithMessage("Company ID must be greater than 0");

		RuleFor(x => x.BankAccountId)
			.GreaterThan(0)
			.WithMessage("Bank Account ID must be greater than 0");

		RuleFor(x => x.InvestmentAmount)
			.GreaterThan(0)
			.WithMessage("Investment amount must be greater than 0")
			.LessThanOrEqualTo(999999999.99m)
			.WithMessage("Investment amount cannot exceed 999,999,999.99");
	}
}