using BankOfMyHouse.Domain.Investments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs.Investments;

internal class InvestmentConfiguration : IEntityTypeConfiguration<Investment>
{
	public void Configure(EntityTypeBuilder<Investment> builder)
	{
		builder.ToTable("Investments");

		// Primary key (assuming Company is unique per investment, otherwise add an Id property)
		builder.HasKey(i => i.Id);

		builder.Property(r => r.Id)
			.UseIdentityByDefaultColumn(); // PostgreSQL identity

		// SharesAmount
		builder.Property(i => i.SharesAmount)
		 .IsRequired()
		 .HasColumnType("numeric(18,2)")
		 .HasComment("Amount of shares invested in the company");

		// Configure Company navigation property
		builder
			.HasOne(i => i.Company)
			.WithMany() // If Company has a collection of Investments, use .WithMany(c => c.Investments)
			.IsRequired();
	}
}
