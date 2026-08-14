using KnustHallMaintenance.Data;
using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KnustHallMaintenance.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<ApplicationUser> _hasher;

    public UserService(ApplicationDbContext context, IPasswordHasher<ApplicationUser> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ApplicationUser?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<List<ApplicationUser>> GetMaintenanceStaffAsync()
    {
        return await _context.Users
            .Where(u => u.Role == UserRole.Maintenance)
            .ToListAsync();
    }

    public bool VerifyPassword(ApplicationUser user, string password)
    {
        return _hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
    }
}
