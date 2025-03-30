using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Infrastructure;
using BankOfMyHouse.Infrastructure.Configurations;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseAsyncSeeding(async (context, _, ct) =>
    {

    });
});


builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddApiEndpoints();

builder.Services
  .AddOptions<JwtConfiguration>()
  .BindConfiguration(JwtConfiguration.ConfigurationSection)
  .ValidateDataAnnotations();

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    })
    .AddCookie("Identity.Bearer");

builder.Services.AddAuthorization();

var app = builder.Build();

await using (var serviceScope = app.Services.CreateAsyncScope())
{
    await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
    {
        await dbContext.Database.EnsureCreatedAsync();
    }
}

app.MapIdentityApi<ApplicationUser>();

app.UseFastEndpoints();

app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");

app.MapScalarApiReference(options =>
{
    options
        .WithTitle("BankOfMyAccount API")
        .WithTheme(ScalarTheme.Mars)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseAuthentication();
app.UseAuthorization();



await app.RunAsync();
