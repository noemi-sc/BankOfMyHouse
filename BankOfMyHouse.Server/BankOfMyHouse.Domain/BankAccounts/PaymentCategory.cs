namespace BankOfMyHouse.Domain.BankAccounts;

public sealed class PaymentCategory
{
	// EF Core constructor
	private PaymentCategory() { }

	private PaymentCategory(PaymentCategoryCode code, string name, string description)
	{
		Code = code;
		Name = name;
		Description = description;
		IsActive = true;
	}

	public int Id { get; set; }
	public PaymentCategoryCode Code { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public bool IsActive { get; set; }

	// Navigation properties
	public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

	// Factory methods
	public static PaymentCategory Create(PaymentCategoryCode code, string name, string description)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be empty", nameof(name));

		return new PaymentCategory(code, name, description);
	}

	public void UpdateDetails(string name, string description)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be empty", nameof(name));

		Name = name;
		Description = description;
	}

	public void Deactivate() => IsActive = false;
	public void Activate() => IsActive = true;
}