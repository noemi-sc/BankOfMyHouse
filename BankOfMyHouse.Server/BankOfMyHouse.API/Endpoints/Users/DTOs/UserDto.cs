namespace BankOfMyHouse.API.Endpoints.Users.DTOs;

public sealed record UserDto
{
	public int Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public DateTime? LastLoginAt { get; set; }
	public bool IsActive { get; set; }
	public List<string> Roles { get; set; } = new();
}