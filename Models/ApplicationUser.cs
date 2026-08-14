namespace KnustHallMaintenance.Models;

public enum UserRole
{
    Student,
    Staff,
    Maintenance,
    Admin
}

public class ApplicationUser
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    // Only relevant for Student accounts
    public string? Hall { get; set; }
    public string? RoomNumber { get; set; }

    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
