using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace KnustHallMaintenance.Models.ViewModels;

public class ComplaintCreateViewModel
{
    [Required]
    public ComplaintCategory Category { get; set; }


    [Required]
    [StringLength(
        1000,
        MinimumLength = 10,
        ErrorMessage = "Please give a bit more detail (at least 10 characters)."
    )]
    public string Description { get; set; } = string.Empty;


    [Required]
    public string Hall { get; set; } = string.Empty;


    [Required]
    public string RoomNumber { get; set; } = string.Empty;


    // Optional photo
    public IFormFile? Photo { get; set; }
}