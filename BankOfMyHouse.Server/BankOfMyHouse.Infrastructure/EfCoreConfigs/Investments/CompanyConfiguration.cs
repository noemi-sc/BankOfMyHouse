using BankOfMyHouse.Domain.Investments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs.Investments;

internal class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
	public void Configure(EntityTypeBuilder<Company> builder)
	{
		builder.ToTable("Companies");

		// Primary key (assuming Name is unique, otherwise add an Id property)
		builder.HasKey(c => c.Id);

		builder.Property(c => c.Id)
			.UseIdentityByDefaultColumn();

		builder.Property(c => c.Name)
			.IsRequired()
			.HasMaxLength(100)
			.HasColumnType("varchar(100)")
			.HasComment("Company name");

		builder.HasMany(x => x.StockPriceHistory)
			.WithOne()
			.HasForeignKey(sp => sp.CompanyId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
