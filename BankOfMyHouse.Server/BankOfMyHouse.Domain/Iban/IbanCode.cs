namespace BankOfMyHouse.Domain.Iban;

public sealed record IbanCode
{
	public string Value { get; init; }

	private IbanCode(string value) => Value = value;

	public static IbanCode Create(string iban)
	{
		if (string.IsNullOrWhiteSpace(iban))
			throw new Exception("IBAN cannot be empty");

		return new IbanCode(iban.Trim());
	}
}
