using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Helpers;

public static class StatusBadgeHelper
{
    public static string CssClass(ComplaintStatus status) => status switch
    {
        ComplaintStatus.Submitted => "badge-submitted",
        ComplaintStatus.Assigned => "badge-assigned",
        ComplaintStatus.InProgress => "badge-inprogress",
        ComplaintStatus.Resolved => "badge-resolved",
        _ => "badge-submitted"
    };

    public static string Display(ComplaintStatus status) => status switch
    {
        ComplaintStatus.InProgress => "In Progress",
        _ => status.ToString()
    };
}
