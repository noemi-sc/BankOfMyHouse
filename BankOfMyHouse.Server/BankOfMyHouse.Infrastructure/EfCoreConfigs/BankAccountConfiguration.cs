using BankOfMyHouse.Domain.BankAccounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs;

internal class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
	public void Configure(EntityTypeBuilder<BankAccount> builder)
	{
		builder.ToTable("BankAccounts");

		builder.HasKey(b => b.Id);

		builder.Property(b => b.Id)
			.UseIdentityByDefaultColumn();
				
		builder.Property(b => b.UserId)
			.IsRequired();

		builder.Property(b => b.CreationDate)
			.IsRequired();

		builder.Property(b => b.Balance)
			.IsRequired();

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
			.WithOne()
			.HasForeignKey("BankAccountId")
			.OnDelete(DeleteBehavior.Cascade);
	}
}
