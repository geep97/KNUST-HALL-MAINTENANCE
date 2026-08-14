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

    Task<List<ApplicationUser>> GetMaintenanceStaffAsync(string hall);


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
    // UPDATE USER
    // ============================================================

    Task<bool> UpdateAsync(
        int id,
        string fullName,
        string email,
        UserRole role,
        string? hall,
        MaintenanceSpecialization? maintenanceSpecialization,
        bool isActive);


    // ============================================================
    // ACTIVATE / DEACTIVATE
    // ============================================================

    Task<bool> SetActiveAsync(
        int id,
        bool isActive);


    // ============================================================
    // DELETE USER
    // ============================================================

    Task<bool> DeleteAsync(
        int id);


    // ============================================================
    // PASSWORD VERIFICATION
    // ============================================================

    bool VerifyPassword(
        ApplicationUser user,
        string password);
}