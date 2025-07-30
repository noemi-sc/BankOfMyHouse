namespace BankOfMyHouse.Domain.Users;

public class UserRole
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTimeOffset AssignedAt { get; set; }
    public int? AssignedBy { get; set; } // User ID who assigned the role
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}