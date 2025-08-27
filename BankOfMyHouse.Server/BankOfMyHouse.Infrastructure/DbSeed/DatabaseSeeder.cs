using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Investments;
using BankOfMyHouse.Domain.Users;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.Infrastructure.DbSeed
{
	public static class DatabaseSeeder
	{
		public static async Task SeedUsers(DbContext context, CancellationToken ct)
		{
			var users = new List<User>
		{
			new User("username1", "user1@gmail.com", "Password1!", "FirstUserName", "FirstUserSurname"),
			new User("username2", "user2@gmail.com", "Password1!", "SecondUserName", "SecondUserSurname"),
			new User("username3", "user3@gmail.com", "Password1!", "ThirdUserName", "ThirdUserSurname"),
		};

			//Assign the user the basic BankAccount role
			var bankAccountRole = await context.Set<Role>()
					.FirstOrDefaultAsync(r => r.Id == 2, ct);

			foreach (var user in users)
			{
				user.UserRoles = new List<UserRole>
					{
					new UserRole
						{
							UserId = user.Id,
							RoleId = bankAccountRole.Id
						}
					};

				var existingUser = await context.Set<User>()
					.FirstOrDefaultAsync(r => r.Id == user.Id, ct);

				if (existingUser == null)
				{
					await context.Set<User>().AddAsync(user, ct);
				}
				else
				{
					context.Set<User>().Update(existingUser);
				}
			}

			await context.SaveChangesAsync(ct);
		}

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

		public static async Task SeedCurrencies(DbContext context, CancellationToken ct)
		{
			var currencies = new List<Currency>
			{
				new("US Dollar", "USD", "$"),
				new("Euro", "EUR", "€"),
				new("British Pound", "GBP", "£"),
				new("Japanese Yen", "JPY", "¥"),
				new("Canadian Dollar", "CAD", "C$"),
				new("Australian Dollar", "AUD", "A$"),
				new("Swiss Franc", "CHF", "CHF"),
				new("Chinese Yuan", "CNY", "¥"),
				new("Swedish Krona", "SEK", "kr"),
				new("Norwegian Krone", "NOK", "kr"),
				new("Danish Krone", "DKK", "kr"),
				new("Polish Zloty", "PLN", "zł"),
				new("Czech Koruna", "CZK", "Kč"),
				new("Hungarian Forint", "HUF", "Ft"),
				new("Romanian Leu", "RON", "lei")
			};

			foreach (var currency in currencies)
			{
				var existingCurrency = await context.Set<Currency>()
					.FirstOrDefaultAsync(c => c.Code == currency.Code, ct);

				if (existingCurrency == null)
				{
					await context.Set<Currency>().AddAsync(currency, ct);
				}
			}

			await context.SaveChangesAsync(ct);
		}

		public static async Task SeedPaymentCategories(DbContext context, CancellationToken ct)
		{
			var paymentCategories = new List<PaymentCategory>
			{
				PaymentCategory.Create(PaymentCategoryCode.Other, "Other", "General payment category for uncategorized transactions"),
				PaymentCategory.Create(PaymentCategoryCode.Shopping, "Shopping", "Purchases of goods and services"),
				PaymentCategory.Create(PaymentCategoryCode.Food, "Food & Dining", "Food purchases and restaurant expenses"),
				PaymentCategory.Create(PaymentCategoryCode.Transport, "Transportation", "Public transport, taxi, fuel costs"),
				PaymentCategory.Create(PaymentCategoryCode.Utilities, "Utilities", "Electricity, water, gas, phone bills"),
				PaymentCategory.Create(PaymentCategoryCode.Entertainment, "Entertainment", "Movies, games, subscriptions"),
				PaymentCategory.Create(PaymentCategoryCode.Healthcare, "Healthcare", "Medical expenses and health insurance"),
				PaymentCategory.Create(PaymentCategoryCode.Education, "Education", "School fees, courses, books"),
				PaymentCategory.Create(PaymentCategoryCode.Salary, "Salary", "Income from employment"),
				PaymentCategory.Create(PaymentCategoryCode.Investment, "Investment", "Investment-related transactions"),
				PaymentCategory.Create(PaymentCategoryCode.Savings, "Savings", "Transfers to savings accounts"),
				PaymentCategory.Create(PaymentCategoryCode.Rent, "Rent/Mortgage", "Housing expenses"),
				PaymentCategory.Create(PaymentCategoryCode.Insurance, "Insurance", "Insurance premiums")
			};

			foreach (var paymentCategory in paymentCategories)
			{
				var existingCategory = await context.Set<PaymentCategory>()
					.FirstOrDefaultAsync(pc => pc.Code == paymentCategory.Code, ct);

				if (existingCategory == null)
				{
					await context.Set<PaymentCategory>().AddAsync(paymentCategory, ct);
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
