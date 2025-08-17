using BankOfMyHouse.Domain.BankAccounts;

namespace BankOfMyHouse.API.Endpoints.PaymentCategories.GetPaymentCategories;

public sealed record GetPaymentCategoriesResponseDto
{
	public List<PaymentCategoryDto> Categories { get; set; } = new();
}

public sealed record PaymentCategoryDto
{
	public int Id { get; set; }
	public PaymentCategoryCode Code { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public bool IsActive { get; set; }
}