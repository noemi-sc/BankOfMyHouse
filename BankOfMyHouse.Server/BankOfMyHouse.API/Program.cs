using BankOfMyHouse.API.Handlers;
using BankOfMyHouse.API.Mappers;
using BankOfMyHouse.Application.Configurations;
using BankOfMyHouse.Application.Hubs;
using BankOfMyHouse.Application.Services.Accounts;
using BankOfMyHouse.Application.Services.Accounts.Interfaces;
using BankOfMyHouse.Application.Services.Investments;
using BankOfMyHouse.Application.Services.Investments.Interfaces;
using BankOfMyHouse.Application.Services.Users;
using BankOfMyHouse.Application.Services.Users.Interfaces;
using BankOfMyHouse.Domain.Iban;
using BankOfMyHouse.Domain.Iban.Interfaces;
using BankOfMyHouse.Domain.Repositories;
using BankOfMyHouse.Infrastructure.Repositories;
using BankOfMyHouse.Infrastructure;
using BankOfMyHouse.Infrastructure.DbSeed;
using FastEndpoints;
using FastEndpoints.Swagger;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using BankOfMyHouse.Application.BackgroundTasks;

var builder = WebApplication.CreateSlimBuilder(args);

new MappingConfig().Register(TypeAdapterConfig.GlobalSettings);

builder.Services.AddSignalR(options =>
{
	options.EnableDetailedErrors = true;
});

builder.Services.AddDbContext<BankOfMyHouseDbContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
	.UseAsyncSeeding(async (context, _, ct) =>
	{
		await DatabaseSeeder.SeedRoles(context, ct);
		await DatabaseSeeder.SeedUsers(context, ct);
		await DatabaseSeeder.SeedPaymentCategories(context, ct);
		await DatabaseSeeder.SeedCurrencies(context, ct);
		await DatabaseSeeder.SeedCompanies(context, ct);
		await DatabaseSeeder.SeedHistoricalStockPrices(context, ct);
	})
	.UseSeeding((context, ct) =>
	{
	});
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));

// Add repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IPaymentCategoryRepository, PaymentCategoryRepository>();

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IBankAccountService, BankAccountService>();
builder.Services.AddScoped<IIbanGenerator, ItalianIbanGenerator>();
builder.Services.AddScoped<IInvestmentService, InvestmentService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings!.Secret);

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ValidateIssuer = true,
		ValidIssuer = jwtSettings.Issuer,
		ValidateAudience = true,
		ValidAudience = jwtSettings.Audience,
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	};
});

builder.Services.AddAuthorizationBuilder()
	.SetDefaultPolicy(new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.Build())
	.AddPolicy("Admin", policy =>
		policy.RequireRole("Admin"))
	.AddPolicy("BankUser", policy =>
		policy.RequireRole("BankUser"));

builder.Services
	.AddFastEndpoints()
	.SwaggerDocument();

// Add CORS if needed
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.WithOrigins("http://localhost:4200")
			  .AllowAnyMethod()
			  .AllowAnyHeader()
			  .AllowCredentials();
	});
});

builder.Services.AddHostedService<StockPriceGenerator>();

var app = builder.Build();

await using (var serviceScope = app.Services.CreateAsyncScope())
{
	await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<BankOfMyHouseDbContext>())
	{
		await dbContext.Database.MigrateAsync();
		dbContext.Database.Migrate();

		//await dbContext.Database.EnsureCreatedAsync();
		//dbContext.Database.EnsureCreated();
		//DO NOT USE ENSURE ETC BECAUSE IT BYPASSES THE MIGRATION PATTERN COMPLETELY!!!!
	}
}

app.UseExceptionHandler();

app.UseCors("AllowAll");
app.UseRouting();

app.MapHub<InvestmentHub>(InvestmentHub.Url);

app.UseAuthentication()
	.UseAuthorization()
	.UseFastEndpoints();

app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");

app.MapScalarApiReference(options =>
{
	options
		.WithTitle("BankOfMyAccount API")
		.WithTheme(ScalarTheme.Mars)
		.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});




await app.RunAsync();

public partial class Program { }

