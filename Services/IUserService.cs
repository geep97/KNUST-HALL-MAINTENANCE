using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetByEmailAsync(string email);

    Task<ApplicationUser?> GetByIdAsync(int id);

    Task<List<ApplicationUser>> GetAllAsync();

    Task<List<ApplicationUser>> GetMaintenanceStaffAsync();

    Task<ApplicationUser> CreateAsync(
        string fullName,
        string email,
        string password,
        UserRole role,
        string? hall = null
    );

    bool VerifyPassword(
        ApplicationUser user,
        string password
    );
}