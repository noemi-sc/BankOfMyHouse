using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Investments;
using BankOfMyHouse.Domain.Users;
using BankOfMyHouse.Infrastructure.EfCoreConfigs.BankAccounts;
using BankOfMyHouse.Infrastructure.EfCoreConfigs.Investments;
using BankOfMyHouse.Infrastructure.EfCoreConfigs.Users;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.Infrastructure;

public class BankOfMyHouseDbContext : DbContext
{
	public virtual DbSet<Role> Roles { get; init; }
	public virtual DbSet<User> Users { get; init; }
	public virtual DbSet<UserRole> UserRoles { get; init; }
	public virtual DbSet<BankAccount> BankAccounts { get; init; }
	public virtual DbSet<Transaction> Transactions { get; init; }
	public virtual DbSet<Currency> Currencies { get; init; }
	public virtual DbSet<PaymentCategory> PaymentCategories { get; init; }
	public virtual DbSet<Company> Companies { get; init; }
	public virtual DbSet<Investment> Investments { get; init; }

	public BankOfMyHouseDbContext(DbContextOptions<BankOfMyHouseDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder
			.ApplyConfiguration(new RoleConfiguration())
			.ApplyConfiguration(new UserConfiguration())
			.ApplyConfiguration(new UserRoleConfiguration())
			.ApplyConfiguration(new BankAccountConfiguration())
			.ApplyConfiguration(new TransactionConfiguration())
			.ApplyConfiguration(new CurrencyConfiguration())
			.ApplyConfiguration(new PaymentCategoryConfiguration())
			.ApplyConfiguration(new InvestmentConfiguration())
			.ApplyConfiguration(new CompanyConfiguration())
			.ApplyConfiguration(new CompanyStockPriceConfiguration());

		base.OnModelCreating(modelBuilder);
	}
}
