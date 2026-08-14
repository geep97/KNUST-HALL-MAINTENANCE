namespace KnustHallMaintenance.Models;

public class Complaint
{
    public int Id { get; set; }


    // =========================================================
    // STUDENT
    // =========================================================

    public int StudentId { get; set; }

    public Student? Student { get; set; }


    // =========================================================
    // COMPLAINT DETAILS
    // =========================================================

    public ComplaintCategory Category { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Hall { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;

    public string? PhotoPath { get; set; }


    // =========================================================
    // STATUS
    // =========================================================

    public ComplaintStatus Status { get; set; }
        = ComplaintStatus.Submitted;


    // =========================================================
    // ASSIGNMENT
    // =========================================================

    public int? AssignedToId { get; set; }

    public ApplicationUser? AssignedTo { get; set; }


    // =========================================================
    // DATES
    // =========================================================

    public DateTime DateCreated { get; set; }
        = DateTime.Now;

    public DateTime? DateResolved { get; set; }


    // =========================================================
    // UPDATES
    // =========================================================

    public ICollection<ComplaintUpdate> Updates { get; set; }
        = new List<ComplaintUpdate>();
}