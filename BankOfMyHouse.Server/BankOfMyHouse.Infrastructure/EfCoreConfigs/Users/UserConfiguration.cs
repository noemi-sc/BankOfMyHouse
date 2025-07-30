using BankOfMyHouse.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs.Users;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("Users");

		// Primary key
		builder.HasKey(u => u.Id)
			.HasName("PK_Users");

		// Properties configuration
		builder.Property(u => u.Id)
			.HasColumnName("UserId")
			.ValueGeneratedOnAdd();

		builder.Property(u => u.Username)
			.IsRequired()
			.HasMaxLength(50)
			.HasColumnName("Username");  // Remove HasColumnType - let EF choose

		builder.Property(u => u.Email)
			.IsRequired()
			.HasMaxLength(255)
			.HasColumnName("Email");     // Remove HasColumnType

		builder.Property(u => u.PasswordHash)
			.IsRequired()
			.HasMaxLength(500)
			.HasColumnName("PasswordHash"); // Remove HasColumnType

		builder.Property(u => u.CreatedAt)
			.IsRequired()
			.HasDefaultValueSql("NOW()")    // PostgreSQL function
			.HasColumnName("CreatedAt");    // Remove HasColumnType

		builder.Property(u => u.LastLoginAt)
			.IsRequired(false)
			.HasColumnName("LastLoginAt");  // Remove HasColumnType

		builder.Property(u => u.IsActive)
			.IsRequired()
			.HasDefaultValue(true)
			.HasColumnName("IsActive");

		builder.HasMany(x => x.UserRoles)
			.WithOne(ur => ur.User)
			.HasForeignKey(ur => ur.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}