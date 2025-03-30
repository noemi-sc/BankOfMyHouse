namespace BankOfMyHouse.Domain.Iban;

public sealed record IbanCode
{
    public string Value { get; }

    private IbanCode(string value) => Value = value;

    public static IbanCode Create(string iban)
    {
        if (string.IsNullOrWhiteSpace(iban))
            throw new Exception("IBAN cannot be empty");

        //if (!ItalianIbanValidator.IsValid(iban))
        //    return Result.Failure<IbanCode>("Invalid IBAN format");

        return new IbanCode(iban);
    }
}
