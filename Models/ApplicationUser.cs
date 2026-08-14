using System.Collections.Generic;

namespace KnustHallMaintenance.Models;

public class ApplicationUser
{
    public int Id { get; set; }


    // =========================================================
    // BASIC USER INFORMATION
    // =========================================================

    public string FullName { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;


    // =========================================================
    // AUTHENTICATION
    // =========================================================

    public string PasswordHash { get; set; }
        = string.Empty;

    public UserRole Role { get; set; }


    // =========================================================
    // LOCATION
    // =========================================================

    public string? Hall { get; set; }

    public string? RoomNumber { get; set; }


    // =========================================================
    // COMPLAINTS ASSIGNED TO THIS USER
    // =========================================================

    public ICollection<Complaint> AssignedComplaints { get; set; }
        = new List<Complaint>();


    // =========================================================
    // COMPLAINT UPDATES MADE BY THIS USER
    // =========================================================

    public ICollection<ComplaintUpdate> ComplaintUpdates { get; set; }
        = new List<ComplaintUpdate>();
}