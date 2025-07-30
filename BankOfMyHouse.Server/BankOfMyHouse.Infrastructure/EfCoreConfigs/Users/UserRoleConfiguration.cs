using BankOfMyHouse.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs.Users;

internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
	public void Configure(EntityTypeBuilder<UserRole> builder)
	{
		// Table configuration
		builder.ToTable("UserRoles");

		// Composite primary key
		builder.HasKey(ur => new { ur.UserId, ur.RoleId })
			.HasName("PK_UserRoles");

		// Properties configuration
		builder.Property(ur => ur.UserId)
			.IsRequired()
			.HasColumnName("UserId");

		builder.Property(ur => ur.RoleId)
			.IsRequired()
			.HasColumnName("RoleId");

		builder.Property(ur => ur.AssignedAt)
			.IsRequired()
			.HasDefaultValueSql("NOW()")
			.HasColumnName("AssignedAt");

		builder.Property(ur => ur.AssignedBy)
			.IsRequired(false)
			.HasColumnName("AssignedBy");

		// Foreign key relationships
		builder.HasOne(ur => ur.User)
			.WithMany(u => u.UserRoles)
			.HasForeignKey(ur => ur.UserId)
			.OnDelete(DeleteBehavior.Cascade)
			.HasConstraintName("FK_UserRoles_Users_UserId");

		builder.HasOne(ur => ur.Role)
			.WithMany(r => r.UserRoles)
			.HasForeignKey(ur => ur.RoleId)
			.OnDelete(DeleteBehavior.Cascade)
			.HasConstraintName("FK_UserRoles_Roles_RoleId");
		// Seed data (optional)
		//builder.HasData(
		//	new UserRole
		//	{
		//		UserId = 1,
		//		RoleId = 1,
		//		AssignedAt = DateTime.UtcNow,
		//		AssignedBy = null // System assigned
		//	},
		//	new UserRole
		//	{
		//		UserId = 1,
		//		RoleId = 2,
		//		AssignedAt = DateTime.UtcNow,
		//		AssignedBy = null
		//	},
		//	new UserRole
		//	{
		//		UserId = 2,
		//		RoleId = 2,
		//		AssignedAt = DateTime.UtcNow,
		//		AssignedBy = 1 // Assigned by admin (user ID 1)
		//	},
		//	new UserRole
		//	{
		//		UserId = 3,
		//		RoleId = 2,
		//		AssignedAt = DateTime.UtcNow,
		//		AssignedBy = 1
		//	},
		//	new UserRole
		//	{
		//		UserId = 3,
		//		RoleId = 3,
		//		AssignedAt = DateTime.UtcNow,
		//		AssignedBy = 1
		//	}
		
	}
}
