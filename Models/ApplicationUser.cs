using Microsoft.AspNetCore.Identity;

namespace KnustHallMaintenance.Models;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string? Hall { get; set; }

    public string? RoomNumber { get; set; }

    public MaintenanceSpecialization? MaintenanceSpecialization { get; set; }

    public ICollection<Complaint> AssignedComplaints { get; set; }
        = new List<Complaint>();
}