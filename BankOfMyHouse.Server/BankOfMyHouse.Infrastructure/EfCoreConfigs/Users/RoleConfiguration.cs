using BankOfMyHouse.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankOfMyHouse.Infrastructure.EfCoreConfigs.Users;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		// Table configuration
		builder.ToTable("Roles");

		// Primary key
		builder.HasKey(r => r.Id);

		// Properties configuration
		builder.Property(r => r.Id)
			.HasColumnName("RoleId")
			.UseIdentityByDefaultColumn(); // PostgreSQL identity

		builder.Property(r => r.Name)
			.IsRequired()
			.HasMaxLength(50)
			.HasColumnType("varchar(50)");

		builder.Property(r => r.Description)
			.HasMaxLength(255)
			.HasColumnType("varchar(255)");

		builder.Property(r => r.CreatedAt)
			.HasDefaultValueSql("NOW()")
			.HasColumnType("timestamp");

		builder.HasMany(x => x.UserRoles)
			.WithOne(ur => ur.Role)
			.HasForeignKey(ur => ur.RoleId)
			.OnDelete(DeleteBehavior.Cascade);

		//// Seed data
		//builder.HasData(
		//	new Role
		//	{
		//		Id = 1,
		//		Name = "Admin",
		//		Description = "System Administrator with full access",
		//		CreatedAt = DateTime.UtcNow
		//	},
		//	new Role
		//	{
		//		Id = 2,
		//		Name = "User",
		//		Description = "Standard user with basic access",
		//		CreatedAt = DateTime.UtcNow
		//	},
		//	new Role
		//	{
		//		Id = 3,
		//		Name = "Moderator",
		//		Description = "Content moderator with elevated privileges",
		//		CreatedAt = DateTime.UtcNow
		//	},
		//	new Role
		//	{
		//		Id = 4,
		//		Name = "Manager",
		//		Description = "Manager with team management capabilities",
		//		CreatedAt = DateTime.UtcNow
		//	}
		//);
	}
}
