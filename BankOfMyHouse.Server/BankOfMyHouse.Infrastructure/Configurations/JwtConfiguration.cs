namespace BankOfMyHouse.Infrastructure.Configurations;

public class JwtConfiguration
{
    public const string ConfigurationSection = "Jwt";

    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Secret { get; init; }
}
