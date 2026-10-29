using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetByEmailAsync(
        string email);

    Task<ApplicationUser?> GetByIdAsync(
        int id);

    Task<List<ApplicationUser>> GetAllAsync();

    Task<List<ApplicationUser>> GetMaintenanceStaffAsync(
        string hall);

    Task<ApplicationUser> CreateAsync(
        string fullName,
        string email,
        string password,
        UserRole role,
        string? hall,
        MaintenanceSpecialization? maintenanceSpecialization);

    Task<bool> UpdateAsync(
        int id,
        string fullName,
        string email,
        UserRole role,
        string? hall,
        MaintenanceSpecialization? maintenanceSpecialization,
        bool isActive);

    Task<bool> SetActiveAsync(
        int id,
        bool isActive);

    Task<bool> DeleteAsync(
        int id);

    Task<bool> IsInRoleAsync(
        ApplicationUser user,
        UserRole role);
}