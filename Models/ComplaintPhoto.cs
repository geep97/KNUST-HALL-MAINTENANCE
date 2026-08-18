namespace KnustHallMaintenance.Models;

public class ComplaintPhoto
{
    public int Id { get; set; }

    public int ComplaintId { get; set; }
    public Complaint Complaint { get; set; } = null!;

    public string PhotoPath { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.Now;
}