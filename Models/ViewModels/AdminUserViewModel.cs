using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Models.ViewModels;

public class AdminUserViewModel
{
    public ApplicationUser User { get; set; } = null!;

    public UserRole Role { get; set; }
}