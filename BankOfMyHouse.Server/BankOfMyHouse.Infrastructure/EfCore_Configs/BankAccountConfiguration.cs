using BankOfMyHouse.Domain.BankAccounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.Configurations
{
    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
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
                .HasMaxLength(50);

            builder.Property(b => b.CreationDate)
                .IsRequired();

            // Configure the IBAN property
            builder.OwnsOne(b => b.IBAN, iban =>
            {
                iban.Property(i => i.Value)
                    .IsRequired()
                    .HasMaxLength(34)
                    .HasColumnName("IBAN");
            });

            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
