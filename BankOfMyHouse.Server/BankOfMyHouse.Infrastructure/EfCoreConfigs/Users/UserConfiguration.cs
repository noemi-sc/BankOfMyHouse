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
			.HasColumnType("nvarchar(50)")
			.HasColumnName("Username");

		builder.Property(u => u.Email)
			.IsRequired()
			.HasMaxLength(255)
			.HasColumnType("nvarchar(255)")
			.HasColumnName("Email");

		builder.Property(u => u.PasswordHash)
			.IsRequired()
			.HasMaxLength(500)
			.HasColumnType("nvarchar(500)")
			.HasColumnName("PasswordHash");

		builder.Property(u => u.CreatedAt)
			.IsRequired()
			.HasDefaultValueSql("GETUTCDATE()")
			.HasColumnType("datetime2")
			.HasColumnName("CreatedAt");

		builder.Property(u => u.LastLoginAt)
			.IsRequired(false)
			.HasColumnType("datetime2")
			.HasColumnName("LastLoginAt");

		builder.Property(u => u.IsActive)
			.IsRequired()
			.HasDefaultValue(true)
			.HasColumnName("IsActive");

		//HERE I CAN SIMPLIFY BY REFERENCING USER ROLE ONLY
		// Many-to-many relationship configuration with Role through UserRole
		builder.HasMany(u => u.Roles)
			.WithMany(r => r.Users)
			.UsingEntity<UserRole>(
				// Configure the right side of the relationship (UserRole -> Role)
				j => j.HasOne(ur => ur.Role)
					.WithMany(r => r.UserRoles)
					.HasForeignKey(ur => ur.RoleId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_UserRoles_Roles_RoleId"),
				// Configure the left side of the relationship (UserRole -> User)
				j => j.HasOne(ur => ur.User)
					.WithMany(u => u.UserRoles)
					.HasForeignKey(ur => ur.UserId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_UserRoles_Users_UserId"),
				// Configure the join entity itself
				j =>
				{
					j.ToTable("UserRoles");
					j.HasKey(ur => new { ur.UserId, ur.RoleId })
						.HasName("PK_UserRoles");
					j.Property(ur => ur.AssignedAt)
						.HasDefaultValueSql("GETUTCDATE()")
						.HasColumnType("datetime2");
					j.Property(ur => ur.AssignedBy)
						.IsRequired(false);
				});
	}
}