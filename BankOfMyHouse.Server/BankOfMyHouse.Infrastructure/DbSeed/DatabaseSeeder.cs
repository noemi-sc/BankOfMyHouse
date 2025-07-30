using BankOfMyHouse.Domain.Investments;
using BankOfMyHouse.Domain.Users;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.Infrastructure.DbSeed
{
	public static class DatabaseSeeder
	{
		public static async Task SeedRoles(DbContext context, CancellationToken ct)
		{
			var roles = new List<Role>
		{
			new Role(1, "Admin", "System Administrator with full access"),
			new Role(2,"BankUser", "Standard user with basic access")
		};

			foreach (var role in roles)
			{
				var existingRole = await context.Set<Role>()
					.FirstOrDefaultAsync(r => r.Id == role.Id, ct);

				if (existingRole == null)
				{
					await context.Set<Role>().AddAsync(role, ct);
				}
			}

			await context.SaveChangesAsync(ct);
		}

		public static async Task SeedCompanies(DbContext context, CancellationToken ct)
		{
			var faker = new Faker<Company>()
				.RuleFor(c => c.Id, f => f.IndexFaker + 1)  // Generates an id starting from 1 instead of 0
				.RuleFor(c => c.Name, f => f.Company.CompanyName());

			var companies = faker.Generate(10);

			foreach (var company in companies)
			{
				var existingCompany = await context.Set<Company>()
					.FirstOrDefaultAsync(r => r.Id == company.Id && r.Name != company.Name, ct);

				if (existingCompany == null)
				{
					await context.Set<Company>().AddAsync(company, ct);
				}
			}

			await context.SaveChangesAsync(ct);
		}
	}
}
