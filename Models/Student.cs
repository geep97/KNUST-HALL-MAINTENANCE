using System.ComponentModel.DataAnnotations;

namespace KnustHallMaintenance.Models;

public class Student
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Programme { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Department { get; set; }

    [MaxLength(150)]
    public string? College { get; set; }

    public int Level { get; set; }

    [Required]
    [MaxLength(100)]
    public string Hall { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string RoomNumber { get; set; } = string.Empty;
}