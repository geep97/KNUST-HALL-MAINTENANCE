using System.ComponentModel.DataAnnotations;
using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Models.ViewModels;

public class AdminCreateUserViewModel
{
    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;


    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;


    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;


    [Required]
    public UserRole Role { get; set; }


    // Required for Hall Staff and Maintenance
    public string? Hall { get; set; }


    // Required only for Maintenance
    [Display(Name = "Maintenance specialization")]
    public MaintenanceSpecialization? MaintenanceSpecialization { get; set; }
}