using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs.BankAccounts;

internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
	public void Configure(EntityTypeBuilder<Transaction> builder)
	{
		builder.ToTable("Transactions");

		// Primary key
		builder.HasKey(t => t.Id);

		// Properties configuration
		builder.Property(t => t.Id)
			.IsRequired()
			.ValueGeneratedOnAdd();

		// Amount - Use decimal with precision for monetary values
		builder.Property(t => t.Amount)
			.IsRequired()
			.HasColumnType("numeric(18,2)") // PostgreSQL numeric, not decimal
			.HasComment("Transaction amount in the account's base currency");

		// Transaction creation timestamp
		builder.Property(t => t.TransactionCreation)
			.IsRequired()
			.HasColumnType("timestamp with time zone") // PostgreSQL timestamp
			.HasDefaultValueSql("NOW()") // PostgreSQL function
			.HasComment("UTC timestamp when transaction was created");

		// PaymentCategory foreign key
		builder.Property(t => t.PaymentCategoryId)
			.IsRequired()
			.HasComment("Foreign key to payment category");

		// PaymentCategory navigation property
		builder.HasOne(t => t.PaymentCategory)
			.WithMany(pc => pc.Transactions)
			.HasForeignKey(t => t.PaymentCategoryId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Restrict);

		// Sender IBAN - simple conversion
		builder.Property(t => t.Sender)
			.HasConversion(
				iban => iban.Value,  // Convert to string for storage
				value => IbanCode.Create(value))  // Convert from string to IbanCode
			.HasColumnName("SenderIban")
			.HasMaxLength(34)
			.IsRequired()
			.HasComment("Sender's IBAN code");

		// Receiver IBAN - simple conversion
		builder.Property(t => t.Receiver)
			.HasConversion(
				iban => iban.Value,  // Convert to string for storage
				value => IbanCode.Create(value))  // Convert from string to IbanCode
			.HasColumnName("ReceiverIban")
			.HasMaxLength(34)
			.IsRequired()
			.HasComment("Receiver's IBAN code");

		builder.Property(t => t.Description)
		   .HasMaxLength(500) // Adjust length as needed
		   .IsRequired(false)
		   .HasComment("Optional transaction description");

		// Currency as foreign key relationship
		builder.HasOne(t => t.Currency)
			.WithMany()
			.HasForeignKey("CurrencyId")
			.IsRequired()
			.OnDelete(DeleteBehavior.Restrict);
	}
}
