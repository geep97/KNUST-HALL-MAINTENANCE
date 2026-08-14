using KnustHallMaintenance.Data;
using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KnustHallMaintenance.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<ApplicationUser> _hasher;

    public UserService(
        ApplicationDbContext context,
        IPasswordHasher<ApplicationUser> hasher)
    {
        _context = context;
        _hasher = hasher;
    }


    // ============================================================
    // GET USER BY EMAIL
    // ============================================================

    public async Task<ApplicationUser?> GetByEmailAsync(
        string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }


    // ============================================================
    // GET USER BY ID
    // ============================================================

    public async Task<ApplicationUser?> GetByIdAsync(
        int id)
    {
        return await _context.Users.FindAsync(id);
    }


    // ============================================================
    // GET ALL PORTAL USERS
    // ============================================================

    public async Task<List<ApplicationUser>> GetAllAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Role)
            .ThenBy(u => u.Hall)
            .ThenBy(u => u.FullName)
            .ToListAsync();
    }


    // ============================================================
    // GET MAINTENANCE PERSONNEL
    // ============================================================

    public async Task<List<ApplicationUser>> GetMaintenanceStaffAsync()
    {
        return await _context.Users
            .Where(u => u.Role == UserRole.Maintenance)
            .OrderBy(u => u.Hall)
            .ThenBy(u => u.FullName)
            .ToListAsync();
    }


    // ============================================================
    // CREATE PORTAL USER
    // ============================================================

    public async Task<ApplicationUser> CreateAsync(
        string fullName,
        string email,
        string password,
        UserRole role,
        string? hall = null)
    {
        var user = new ApplicationUser
        {
            FullName = fullName,
            Email = email,
            Role = role,

            // Admin has no hall.
            // Hall Staff and Maintenance have their assigned hall.
            Hall = role == UserRole.Admin
                ? null
                : hall
        };


        user.PasswordHash =
            _hasher.HashPassword(
                user,
                password
            );


        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }


    // ============================================================
    // VERIFY PASSWORD
    // ============================================================

    public bool VerifyPassword(
        ApplicationUser user,
        string password)
    {
        var result =
            _hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password
            );

        return result != PasswordVerificationResult.Failed;
    }
}