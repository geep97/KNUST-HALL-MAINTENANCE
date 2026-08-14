using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Services;

public interface IUserService
{
    // ============================================================
    // USER LOOKUPS
    // ============================================================

    Task<ApplicationUser?> GetByEmailAsync(
        string email);

    Task<ApplicationUser?> GetByIdAsync(
        int id);


    // ============================================================
    // USER LISTING
    // ============================================================

    Task<List<ApplicationUser>> GetAllAsync();


    // ============================================================
    // MAINTENANCE PERSONNEL
    // ============================================================

    Task<List<ApplicationUser>> GetMaintenanceStaffAsync();


    // ============================================================
    // CREATE USER
    // ============================================================

    Task<ApplicationUser> CreateAsync(
        string fullName,
        string email,
        string password,
        UserRole role,
        string? hall,
        MaintenanceSpecialization? maintenanceSpecialization);


    // ============================================================
    // PASSWORD VERIFICATION
    // ============================================================

    bool VerifyPassword(
        ApplicationUser user,
        string password);
}