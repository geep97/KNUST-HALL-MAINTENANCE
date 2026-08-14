using System.ComponentModel.DataAnnotations;
using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Models.ViewModels;

public class AdminEditUserViewModel
{
    public int Id { get; set; }


    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;


    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;


    [Required]
    public UserRole Role { get; set; }


    public string? Hall { get; set; }


    [Display(Name = "Maintenance specialization")]
    public MaintenanceSpecialization? MaintenanceSpecialization
    {
        get;
        set;
    }


    public bool IsActive { get; set; }
}