using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Services;

public class ComplaintSlaService
{
    public TimeSpan GetTargetDuration(ComplaintCategory category)
    {
        return category switch
        {
            ComplaintCategory.Electrical => TimeSpan.FromHours(24),
            ComplaintCategory.Plumbing => TimeSpan.FromHours(24),
            ComplaintCategory.Carpentry => TimeSpan.FromHours(48),
            ComplaintCategory.Masonry => TimeSpan.FromHours(72),
            ComplaintCategory.Painting => TimeSpan.FromHours(72),
            ComplaintCategory.Structural => TimeSpan.FromHours(72),
            ComplaintCategory.Furniture => TimeSpan.FromHours(48),
            ComplaintCategory.Other => TimeSpan.FromHours(48),
            _ => TimeSpan.FromHours(48)
        };
    }

    public DateTime CalculateDueAt(
        ComplaintCategory category,
        DateTime assignedAt)
    {
        return assignedAt + GetTargetDuration(category);
    }

    public bool IsOverdue(Complaint complaint)
    {
        if (!complaint.DueAt.HasValue)
            return false;

        if (complaint.Status == ComplaintStatus.Resolved)
            return false;

        return DateTime.Now > complaint.DueAt.Value;
    }

    public bool NeedsAttention(Complaint complaint)
    {
        if (complaint.Status != ComplaintStatus.InProgress)
            return false;

        if (!complaint.LastActivityAt.HasValue)
            return false;

        return DateTime.Now > complaint.LastActivityAt.Value.AddHours(24);
    }
}