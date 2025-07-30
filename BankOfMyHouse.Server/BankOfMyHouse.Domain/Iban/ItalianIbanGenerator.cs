using BankOfMyHouse.Domain.Iban.Interfaces;
using System.Text;

namespace BankOfMyHouse.Domain.Iban;

public class ItalianIbanGenerator : IIbanGenerator
{
    private static readonly Random _random = new Random();
    private const string ABI = "12345";
    private const string CAB = "12345";

    public IbanCode GenerateItalianIban()
    {
        // Generate random account number (1-12 digits)
        string accountNumber = _random.Next(1, 999999999).ToString();

        // Calculate CIN
        char cin = CalculateCin(ABI, CAB);

        // Build BBAN (23 chars)
        string bban = $"{cin}{ABI}{CAB}{accountNumber.PadLeft(12, '0')}";

        // Calculate check digits
        string checkDigits = CalculateCheckDigits(bban);

        // Return full IBAN
        return IbanCode.Create($"IT{checkDigits}{bban}");
    }

    private static char CalculateCin(string abi, string cab)
    {
        int[] weights = { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 };
        int sum = 0;

        for (int i = 0; i < 10; i++)
        {
            int digit = int.Parse((i < 5 ? abi : cab)[i % 5].ToString());
            int product = digit * weights[i];
            sum += (product / 10) + (product % 10);
        }

        int remainder = sum % 10;
        int cinDigit = (10 - remainder) % 10;
        return (char)('A' + cinDigit);
    }

    private static string CalculateCheckDigits(string bban)
    {
        string rearranged = $"{bban}IT00";
        StringBuilder numericString = new StringBuilder();

        foreach (char c in rearranged)
        {
            numericString.Append(char.IsLetter(c)
                ? (c - 'A' + 10).ToString()
                : c.ToString());
        }

        int modResult = Mod97(numericString.ToString());
        return (98 - modResult).ToString("00");
    }

    private static int Mod97(string numericString)
    {
        int current = 0;
        foreach (char c in numericString)
        {
            current = (current * 10 + (c - '0')) % 97;
        }
        return current;
    }

}
