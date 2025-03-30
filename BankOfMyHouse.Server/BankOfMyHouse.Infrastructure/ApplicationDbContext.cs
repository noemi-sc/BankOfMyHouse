using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Users;
using BankOfMyHouse.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<TransactionConfiguration> Transactions { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .ApplyConfiguration(new BankAccountConfiguration())
            .ApplyConfiguration(new TransactionConfiguration());
    }
}
