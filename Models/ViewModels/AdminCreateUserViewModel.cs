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
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Role")]
    public UserRole Role { get; set; }

    public string? Hall { get; set; }

    [Display(Name = "Room number")]
    public string? RoomNumber { get; set; }
}