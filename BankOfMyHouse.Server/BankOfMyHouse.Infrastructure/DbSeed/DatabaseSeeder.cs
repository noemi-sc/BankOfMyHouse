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
			new User("username1", "user1@gmail.com", "Password1!", "FirstUserName", "FirstUserSurname")
			{
				Id = 1
			},
			new User("username2", "user2@gmail.com", "Password1!", "SecondUserName", "SecondUserSurname")
			{
				Id = 2
			},
			new User("username3", "user3@gmail.com", "Password1!", "ThirdUserName", "ThirdUserSurname")
			{
				Id = 3
			},
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
					.FirstOrDefaultAsync(r => r.Username == user.Username && r.Email == user.Email && r.Id == user.Id, ct);

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
			new Role(2, "BankUser", "Standard user with basic access")
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

		//Adds a whole month of data points for each company
		public static async Task SeedHistoricalStockPrices(DbContext context, CancellationToken ct)
		{
			// Skip if data already exists
			if (await context.Set<CompanyStockPrice>().AnyAsync(ct))
				return;

			var companies = await context.Set<Company>().ToListAsync(ct);
			if (companies.Count == 0)
				return;

			var random = new Random(42);

			var now = DateTimeOffset.UtcNow;
			var startOfMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
			var endDate = now.Date.AddDays(-1).AddHours(23).AddMinutes(59);

			const int batchSize = 50_000;
			var stockPrices = new List<CompanyStockPrice>(batchSize);

			foreach (var company in companies)
			{
				// Start with an initial random price
				decimal currentPrice = (decimal)(random.NextDouble() * 190 + 10);
				var currentDate = startOfMonth;

				while (currentDate <= endDate)
				{
					// Random minute-to-minute volatility: -0.5% to +0.5%					
					var percentageChange = (decimal)(random.NextDouble() * 1.0 - 0.5);
					currentPrice *= (1 + percentageChange / 100);

					// Clamp prices between 5 and 500 EUR
					if (currentPrice < 5) currentPrice = 5;
					if (currentPrice > 500) currentPrice = 500;

					var stockPrice = new CompanyStockPrice(Math.Round(currentPrice, 2), company.Id)
					{
						TimeOfPriceChange = currentDate
					};

					stockPrices.Add(stockPrice);

					// Save in batches
					if (stockPrices.Count >= batchSize)
					{

						await context.Set<CompanyStockPrice>().AddRangeAsync(stockPrices, ct);
						await context.SaveChangesAsync(ct);
						stockPrices.Clear();
					}

					// Move forward one minute
					currentDate = currentDate.AddMinutes(1);
				}
			}

			// Save remaining items
			if (stockPrices.Count > 0)
			{
				await context.Set<CompanyStockPrice>().AddRangeAsync(stockPrices, ct);
				await context.SaveChangesAsync(ct);
			}
		}
	}
}
