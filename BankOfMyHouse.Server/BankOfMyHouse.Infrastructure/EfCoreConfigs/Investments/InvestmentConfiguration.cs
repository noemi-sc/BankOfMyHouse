using BankOfMyHouse.Domain.Investments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs.Investments;

internal class InvestmentConfiguration : IEntityTypeConfiguration<Investment>
{
	public void Configure(EntityTypeBuilder<Investment> builder)
	{
		builder.ToTable("Investments");

		builder.HasKey(i => i.Id);

		builder.Property(r => r.Id)
			.UseIdentityByDefaultColumn();

		// SharesAmount
		builder.Property(i => i.SharesAmount)
		 .IsRequired()
		 .HasColumnType("numeric(18,2)")
		 .HasComment("Amount of shares invested in the company");

		// PurchasePrice
		builder.Property(i => i.PurchasePrice)
		 .IsRequired()
		 .HasColumnType("numeric(18,2)")
		 .HasComment("Stock price per share at the time of purchase");

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		// Configure Company navigation property
		builder
			.HasOne(i => i.Company)
			.WithMany()
			.HasForeignKey(x => x.CompanyId)
			.HasConstraintName("FK_Investments_Companies")
			.IsRequired();

		builder
			.HasOne(i => i.BankAccount)
			.WithMany(x => x.Investments)
			.HasForeignKey(x => x.BankAccountId)
			.HasConstraintName("FK_Investments_BankAccounts")
			.IsRequired();
	}
}
