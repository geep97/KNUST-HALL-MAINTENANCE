using System.ComponentModel.DataAnnotations;

namespace KnustHallMaintenance.Models.ViewModels;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Email or Student ID")]
    public string EmailOrStudentId { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}