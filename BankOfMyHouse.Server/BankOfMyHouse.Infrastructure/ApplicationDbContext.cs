using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Users;
using BankOfMyHouse.Infrastructure.EfCoreConfigs;
using BankOfMyHouse.Infrastructure.EfCoreConfigs.Users;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.Infrastructure;

public class ApplicationDbContext : DbContext
{
	public virtual DbSet<Role> Roles { get; init; }
	public virtual DbSet<User> Users { get; init; }	
	public virtual DbSet<UserRole> UserRoles { get; init; }
	public virtual DbSet<BankAccount> BankAccounts { get; init; }
	public virtual DbSet<Transaction> Transactions { get; init; }	

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .ApplyConfiguration(new RoleConfiguration())
            .ApplyConfiguration(new UserConfiguration())
            .ApplyConfiguration(new UserRoleConfiguration())
            .ApplyConfiguration(new BankAccountConfiguration())
            .ApplyConfiguration(new TransactionConfiguration());
    }
}
