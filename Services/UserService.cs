using KnustHallMaintenance.Data;
using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KnustHallMaintenance.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetByEmailAsync(
        string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> GetByIdAsync(
        int id)
    {
        return await _userManager.FindByIdAsync(
            id.ToString()
        );
    }

    public async Task<List<ApplicationUser>> GetAllAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Hall)
            .ThenBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<List<ApplicationUser>>
        GetMaintenanceStaffAsync(string hall)
    {
        if (string.IsNullOrWhiteSpace(hall))
        {
            return new List<ApplicationUser>();
        }

        var users = await _context.Users
            .Where(u =>
                u.IsActive &&
                u.Hall == hall &&
                u.MaintenanceSpecialization != null)
            .OrderBy(u => u.MaintenanceSpecialization)
            .ThenBy(u => u.FullName)
            .ToListAsync();

        var maintenanceStaff =
            new List<ApplicationUser>();

        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(
                user,
                UserRole.Maintenance.ToString()))
            {
                maintenanceStaff.Add(user);
            }
        }

        return maintenanceStaff;
    }

    public async Task<ApplicationUser> CreateAsync(
        string fullName,
        string email,
        string password,
        UserRole role,
        string? hall,
        MaintenanceSpecialization? maintenanceSpecialization)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            Hall = hall,
            IsActive = true,
            MaintenanceSpecialization =
                role == UserRole.Maintenance
                    ? maintenanceSpecialization
                    : null
        };

        var result = await _userManager.CreateAsync(
            user,
            password
        );

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(e => e.Description)
            );

            throw new InvalidOperationException(errors);
        }

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                role.ToString()
            );

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            var errors = string.Join(
                ", ",
                roleResult.Errors.Select(e => e.Description)
            );

            throw new InvalidOperationException(errors);
        }

        return user;
    }

    public async Task<bool> UpdateAsync(
        int id,
        string fullName,
        string email,
        UserRole role,
        string? hall,
        MaintenanceSpecialization? maintenanceSpecialization,
        bool isActive)
    {
        var user =
            await _userManager.FindByIdAsync(
                id.ToString()
            );

        if (user == null)
        {
            return false;
        }

        var currentRoles =
            await _userManager.GetRolesAsync(user);

        var currentRole =
            currentRoles.FirstOrDefault();

        var hallChanged =
            !string.Equals(
                user.Hall,
                hall,
                StringComparison.OrdinalIgnoreCase
            );

        if (
            hallChanged &&
            currentRole == UserRole.Maintenance.ToString()
        )
        {
            var assignedComplaints =
                await _context.Complaints
                    .Where(c =>
                        c.AssignedToId == user.Id &&
                        c.Status != ComplaintStatus.Resolved)
                    .ToListAsync();

            foreach (var complaint in assignedComplaints)
            {
                complaint.AssignedToId = null;
            }
        }

        user.FullName = fullName;
        user.Email = email;
        user.UserName = email;
        user.Hall = hall;
        user.IsActive = isActive;

        user.MaintenanceSpecialization =
            role == UserRole.Maintenance
                ? maintenanceSpecialization
                : null;

        var updateResult =
            await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return false;
        }

        if (currentRoles.Count > 0)
        {
            var removeResult =
                await _userManager.RemoveFromRolesAsync(
                    user,
                    currentRoles
                );

            if (!removeResult.Succeeded)
            {
                return false;
            }
        }

        var addRoleResult =
            await _userManager.AddToRoleAsync(
                user,
                role.ToString()
            );

        if (!addRoleResult.Succeeded)
        {
            return false;
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetActiveAsync(
        int id,
        bool isActive)
    {
        var user =
            await _userManager.FindByIdAsync(
                id.ToString()
            );

        if (user == null)
        {
            return false;
        }

        user.IsActive = isActive;

        var result =
            await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<bool> DeleteAsync(
        int id)
    {
        var user =
            await _userManager.FindByIdAsync(
                id.ToString()
            );

        if (user == null)
        {
            return false;
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        if (roles.Contains(
            UserRole.Admin.ToString()))
        {
            return false;
        }

        var hasAssignedComplaints =
            await _context.Complaints
                .AnyAsync(
                    c => c.AssignedToId == id
                );

        if (hasAssignedComplaints)
        {
            return false;
        }

        var hasComplaintUpdates =
            await _context.ComplaintUpdates
                .AnyAsync(
                    u => u.UpdatedById == id
                );

        if (hasComplaintUpdates)
        {
            return false;
        }

        var result =
            await _userManager.DeleteAsync(user);

        return result.Succeeded;
    }

    public async Task<bool> IsInRoleAsync(
        ApplicationUser user,
        UserRole role)
    {
        return await _userManager.IsInRoleAsync(
            user,
            role.ToString()
        );
    }
}