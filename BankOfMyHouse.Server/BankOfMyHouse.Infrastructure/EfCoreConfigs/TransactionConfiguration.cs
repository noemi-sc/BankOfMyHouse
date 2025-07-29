using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs;

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
			.HasColumnType("timestamptz") // PostgreSQL timestamp
			.HasDefaultValueSql("NOW()") // PostgreSQL function
			.HasComment("UTC timestamp when transaction was created");

		// Payment category enum
		builder.Property(t => t.PaymentCategory)
			.IsRequired()
			.HasConversion<string>()  // Store as string in database
			.HasMaxLength(50);

		builder.Property(t => t.Sender)
				.HasConversion(
					iban => iban.Value,  // Convert to string for storage
					value => IbanCode.Create(value))  // Convert from string to IbanCode
				.HasColumnName("SenderIban")
				.HasMaxLength(34)  // IBAN max length is 34 characters
				.IsRequired()
				.HasComment("Sender's IBAN code");

		// Receiver IBAN
		builder.Property(t => t.Receiver)
			.HasConversion(
				iban => iban.Value,  // Convert to string for storage
				value => IbanCode.Create(value))  // Convert from string to IbanCode
			.HasColumnName("ReceiverIban")
			.HasMaxLength(34)  // IBAN max length is 34 characters
			.IsRequired()
			.HasComment("Receiver's IBAN code");

		// Check constraints
		//builder.ToTable(t => t.HasCheckConstraint("CK_Transaction_Amount_Positive", "[Amount] > 0"));
		//builder.ToTable(t => t.HasCheckConstraint("CK_Transaction_Different_Parties", "[SenderId] != [ReceiverId]"));
	}
}
