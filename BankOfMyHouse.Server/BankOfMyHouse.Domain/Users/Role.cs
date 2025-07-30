namespace BankOfMyHouse.Domain.Users;

public class Role
{
	public Role(int id, string name, string description)
	{
		Id = id;
		Name = name;
		Description = description;
		CreatedAt = DateTime.Now;
	}

	public Role() { }

	public int Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public string Description { get; init; } = string.Empty;
	public DateTime CreatedAt { get; init; }

	// Navigation properties
	public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}