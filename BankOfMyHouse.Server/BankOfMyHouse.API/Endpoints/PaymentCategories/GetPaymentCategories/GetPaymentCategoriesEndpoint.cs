using BankOfMyHouse.Infrastructure;
using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BankOfMyHouse.API.Endpoints.PaymentCategories.GetPaymentCategories;

public class GetPaymentCategoriesEndpoint : EndpointWithoutRequest<GetPaymentCategoriesResponseDto>
{
	private readonly BankOfMyHouseDbContext _dbContext;
	private readonly ILogger<GetPaymentCategoriesEndpoint> _logger;

	public GetPaymentCategoriesEndpoint(
		BankOfMyHouseDbContext dbContext,
		ILogger<GetPaymentCategoriesEndpoint> logger)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public override void Configure()
	{
		Get("/payment-categories");
		Roles("BankUser");
		Summary(s =>
		{
			s.Summary = "Get Payment Categories";
			s.Description = "Retrieve all available payment categories for transactions";
			s.Responses[200] = "Payment categories retrieved successfully";
			s.Responses[500] = "Internal server error";
		});
	}

	public override async Task HandleAsync(CancellationToken ct)
	{
		var paymentCategories = await _dbContext.PaymentCategories
			.Where(pc => pc.IsActive)
			.OrderBy(pc => pc.Name)
			.ToListAsync(ct);

		var response = new GetPaymentCategoriesResponseDto
		{
			Categories = paymentCategories.Adapt<List<PaymentCategoryDto>>()
		};

		await Send.OkAsync(response, ct);
	}
}