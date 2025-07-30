using BankOfMyHouse.Domain.Investments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs.Investments
{
	internal class CompanyStockPriceConfiguration : IEntityTypeConfiguration<CompanyStockPrice>
	{
		public void Configure(EntityTypeBuilder<CompanyStockPrice> builder)
		{
			builder.HasKey(x => new { x.TimeOfPriceChange, x.CompanyId });

			builder.Property(x => x.TimeOfPriceChange);

			builder.Property(x => x.StockPrice);
		}
	}
}
