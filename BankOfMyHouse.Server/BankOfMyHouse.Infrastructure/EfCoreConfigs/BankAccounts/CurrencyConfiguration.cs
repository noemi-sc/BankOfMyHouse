using BankOfMyHouse.Domain.BankAccounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs.BankAccounts;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .UseIdentityColumn();
        
        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(3);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(c => c.Symbol)
            .IsRequired()
            .HasMaxLength(5);
        
        // Create unique index on Code
        builder.HasIndex(c => c.Code)
            .IsUnique();
        
    }
}