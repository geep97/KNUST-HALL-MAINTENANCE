using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<ApplicationUser?> GetByIdAsync(int id);
    Task<List<ApplicationUser>> GetMaintenanceStaffAsync();

    /// <summary>
    /// Verifies a plaintext password against the user's stored hash.
    /// Returns true if it matches.
    /// </summary>
    bool VerifyPassword(ApplicationUser user, string password);
}
