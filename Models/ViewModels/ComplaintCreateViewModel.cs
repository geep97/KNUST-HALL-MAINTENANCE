using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace KnustHallMaintenance.Models.ViewModels;

public class ComplaintCreateViewModel
{
    [Required(ErrorMessage = "Please select a complaint category.")]
    public ComplaintCategory Category { get; set; }

    [Required(ErrorMessage = "Please describe the issue.")]
    [StringLength(
        2000,
        ErrorMessage = "The description cannot exceed 2000 characters."
    )]
    public string Description { get; set; } = string.Empty;

    public string? Hall { get; set; }

    public string? RoomNumber { get; set; }

    public List<IFormFile> Photos { get; set; } = new();
}