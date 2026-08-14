namespace KnustHallMaintenance.Models;

public enum UserRole
{
    Admin = 3,
    HallStaff = 1,
    Maintenance = 2
}

public enum ComplaintCategory
{
    Electrical,
    Plumbing,
    Furniture,
    Structural,
    Other
}

public enum ComplaintStatus
{
    Submitted,
    Assigned,
    InProgress,
    Resolved
}