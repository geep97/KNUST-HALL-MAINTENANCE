namespace KnustHallMaintenance.Models;

public enum UserRole
{
    Admin = 3,
    HallStaff = 1,
    Maintenance = 2
}

public enum MaintenanceSpecialization
{
    Electrical,
    Plumbing,
    Carpentry,
    Masonry,
    Painting,
    GeneralMaintenance
}

public enum ComplaintCategory
{
    Plumbing,
    Electrical,
    Carpentry,
    Masonry,
    Painting,
    Structural,
    Furniture,
    Other
}

public enum ComplaintStatus
{
    Submitted,
    Assigned,
    InProgress,
    Resolved
}