namespace KnustHallMaintenance.Models;

public class ComplaintUpdate
{
    public int Id { get; set; }

    public int ComplaintId { get; set; }
    public Complaint? Complaint { get; set; }

    public int UpdatedById { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }

    public string Note { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
