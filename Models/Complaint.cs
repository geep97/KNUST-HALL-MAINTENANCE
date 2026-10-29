namespace KnustHallMaintenance.Models;

public class Complaint
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public ComplaintCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Hall { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;

    public string? PhotoPath { get; set; }

    public ComplaintStatus Status { get; set; } = ComplaintStatus.Submitted;

    public int? AssignedToId { get; set; }
    public ApplicationUser? AssignedTo { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime? DateResolved { get; set; }

    public DateTime? AssignedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? DueAt { get; set; }
    public DateTime? LastActivityAt { get; set; }

    public bool? ResolutionConfirmed { get; set; }
    public DateTime? ResolutionConfirmedAt { get; set; }

    public ICollection<ComplaintPhoto> Photos { get; set; }
        = new List<ComplaintPhoto>();

    public ICollection<ComplaintUpdate> Updates { get; set; }
        = new List<ComplaintUpdate>();
}