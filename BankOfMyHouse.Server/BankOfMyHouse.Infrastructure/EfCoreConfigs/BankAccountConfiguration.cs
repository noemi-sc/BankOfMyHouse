using BankOfMyHouse.Domain.BankAccounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs;

internal class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
	public void Configure(EntityTypeBuilder<BankAccount> builder)
	{
		// Configure the table name
		builder.ToTable("BankAccounts");

		// Configure the primary key
		builder.HasKey(b => b.Id);

		builder.Property(b => b.Id)
		   .ValueGeneratedOnAdd();

		// Configure properties
		builder.Property(b => b.UserId)
			.IsRequired()
			.HasMaxLength(450)
			.HasColumnType("varchar(450)"); // PostgreSQL varchar

		builder.Property(b => b.CreationDate)
			.IsRequired()
			.HasColumnType("timestamptz") // PostgreSQL timestamp
			.HasDefaultValueSql("NOW()"); // PostgreSQL function

		// Configure the IBAN property
		builder.OwnsOne(b => b.IBAN, iban =>
		{
			iban.Property(i => i.Value)
				.IsRequired()
				.HasMaxLength(34)
				.HasColumnName("IBAN")
				.HasComment("International Bank Account Number");
		});

		// Configure relationship with User
		builder.HasOne(b => b.User)
			.WithMany(x => x.BankAccounts)
			.HasForeignKey(b => b.UserId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

		// Configure relationship with Transactions
		builder.HasMany(b => b.Transactions)
			.WithOne() // No navigation property back to BankAccount in Transaction
			.HasForeignKey("BankAccountId") // Shadow property
			.OnDelete(DeleteBehavior.Cascade);
	}
}
