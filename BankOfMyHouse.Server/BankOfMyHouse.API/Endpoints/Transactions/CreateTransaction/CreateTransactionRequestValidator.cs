using FastEndpoints;
using FluentValidation;

namespace BankOfMyHouse.API.Endpoints.Transactions.CreateTransaction;

public class CreateTransactionRequestValidator : Validator<CreateTransactionRequestDto>
{
	public CreateTransactionRequestValidator()
	{
		RuleFor(x => x.SenderIban)
			.NotNull()
			.WithMessage("Sender IBAN is required");

		RuleFor(x => x.SenderIban.Value)
			.NotEmpty()
			.WithMessage("Sender IBAN cannot be empty")
			.MaximumLength(34)
			.WithMessage("IBAN cannot exceed 34 characters")
			.When(x => x.SenderIban != null);

		RuleFor(x => x.ReceiverIban)
			.NotNull()
			.WithMessage("Receiver IBAN is required");

		RuleFor(x => x.ReceiverIban.Value)
			.NotEmpty()
			.WithMessage("Receiver IBAN cannot be empty")
			.MaximumLength(34)
			.WithMessage("IBAN cannot exceed 34 characters")
			.When(x => x.ReceiverIban != null);

		RuleFor(x => x.Amount)
			.GreaterThan(0)
			.WithMessage("Amount must be greater than 0")
			.LessThanOrEqualTo(999999999.99m)
			.WithMessage("Amount cannot exceed 999,999,999.99");

		RuleFor(x => x.CurrencyCode)
			.NotEmpty()
			.WithMessage("Currency code is required")
			.Length(3)
			.WithMessage("Currency code must be exactly 3 characters")
			.Matches("^[A-Z]{3}$")
			.WithMessage("Currency code must be 3 uppercase letters (e.g., EUR, USD)");

		RuleFor(x => x.Description)
			.MaximumLength(500)
			.WithMessage("Description cannot exceed 500 characters")
			.When(x => !string.IsNullOrEmpty(x.Description));

		RuleFor(x => x.PaymentCategory)
			.IsInEnum()
			.WithMessage("Invalid payment category");

		// Custom validation: sender and receiver cannot be the same
		RuleFor(x => x)
			.Must(x => x.SenderIban?.Value != x.ReceiverIban?.Value)
			.WithMessage("Sender and receiver IBAN cannot be the same")
			.When(x => x.SenderIban != null && x.ReceiverIban != null);
	}
}