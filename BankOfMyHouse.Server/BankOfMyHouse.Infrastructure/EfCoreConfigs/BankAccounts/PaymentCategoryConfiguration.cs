using BankOfMyHouse.Domain.BankAccounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs.BankAccounts;

public class PaymentCategoryConfiguration : IEntityTypeConfiguration<PaymentCategory>
{
    public void Configure(EntityTypeBuilder<PaymentCategory> builder)
    {
        builder.ToTable("payment_categories");

        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Id)
            .HasColumnName("id");

        builder.Property(pc => pc.Code)
            .HasColumnName("code")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(pc => pc.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pc => pc.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(pc => pc.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        // Unique constraint on code
        builder.HasIndex(pc => pc.Code)
            .IsUnique()
            .HasDatabaseName("ix_payment_categories_code");

        // One-to-many relationship with transactions
        builder.HasMany(pc => pc.Transactions)
            .WithOne(t => t.PaymentCategory)
            .HasForeignKey(t => t.PaymentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}