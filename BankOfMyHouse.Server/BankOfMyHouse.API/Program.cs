using BankOfMyHouse.Application.Configurations;
using BankOfMyHouse.Application.Users;
using BankOfMyHouse.Application.Users.Interfaces;
using BankOfMyHouse.Infrastructure;
using FastEndpoints;
using FastEndpoints.Swagger;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Reflection;
using System.Text;
using IMapper = MapsterMapper.IMapper;

var builder = WebApplication.CreateSlimBuilder(args);

// Configure Mapster
var config = new TypeAdapterConfig();
// Scan and register all mapping configurations
config.Scan(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
	.UseAsyncSeeding(async (context, _, ct) =>
	{

	});
});


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();

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
	.AddPolicy("AdminOnly", policy =>
		policy.RequireRole("Admin"))
	.AddPolicy("CustomerUser", policy =>
		policy.RequireRole("CustomerUser"))
	//.AddPolicy("AdminOrSpecificUser", policy =>
	//	policy.RequireAssertion(context =>
	//		context.User.IsInRole("Admin") ||
	//		context.User.Identity?.Name == "specificuser"));

builder.Services
	.AddFastEndpoints()
	.SwaggerDocument();

// Add CORS if needed
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

var app = builder.Build();

await using (var serviceScope = app.Services.CreateAsyncScope())
{
	await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
	{
		//await dbContext.Database.EnsureCreatedAsync();
	}
}

app.UseCors("AllowAll");


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
