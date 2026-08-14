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
            .FirstOrDefaultAsync(
                u => u.Email == email
            );
    }


    // ============================================================
    // GET USER BY ID
    // ============================================================

    public async Task<ApplicationUser?> GetByIdAsync(
        int id)
    {
        return await _context.Users
            .FindAsync(id);
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
    // GET ACTIVE MAINTENANCE PERSONNEL
    // ============================================================

    public async Task<List<ApplicationUser>> GetMaintenanceStaffAsync(
    string hall)
    {
        if (string.IsNullOrWhiteSpace(hall))
        {
            return new List<ApplicationUser>();
        }

        return await _context.Users
            .Where(u =>
                u.Role == UserRole.Maintenance &&
                u.IsActive &&
                u.Hall == hall)
            .OrderBy(u => u.MaintenanceSpecialization)
            .ThenBy(u => u.FullName)
            .ToListAsync();
    }


    // ============================================================
    // CREATE USER
    // ============================================================

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
            FullName = fullName,
            Email = email,
            Role = role,
            Hall = hall,

            // Only Maintenance users should have
            // a maintenance specialization.
            MaintenanceSpecialization =
                role == UserRole.Maintenance
                    ? maintenanceSpecialization
                    : null,

            IsActive = true
        };


        // ========================================================
        // HASH PASSWORD
        // ========================================================

        user.PasswordHash =
            _hasher.HashPassword(
                user,
                password
            );


        // ========================================================
        // SAVE USER
        // ========================================================

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }


    // ============================================================
    // UPDATE USER
    // ============================================================
    //
    // IMPORTANT:
    //
    // If a Maintenance worker is moved to another hall:
    //
    //     Old Hall
    //         ↓
    //     Admin changes Hall
    //         ↓
    //     Existing active assignments are removed
    //         ↓
    //     Complaint status is PRESERVED
    //         ↓
    //     New Hall Staff can reassign the complaint
    //
    // Example:
    //
    //     Technician A
    //     Unity Hall
    //
    //     Complaint
    //     Status = InProgress
    //     AssignedTo = Technician A
    //
    //     Technician A moved to Independence Hall
    //
    //     Complaint
    //     Status = InProgress
    //     AssignedTo = NULL
    //
    // ============================================================

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
            await _context.Users
                .FindAsync(id);

        if (user == null)
        {
            return false;
        }


        // ========================================================
        // DETECT WHETHER THE HALL IS CHANGING
        // ========================================================

        var hallChanged =
            !string.Equals(
                user.Hall,
                hall,
                StringComparison.OrdinalIgnoreCase
            );


        // ========================================================
        // MAINTENANCE WORKER HALL TRANSFER
        // ========================================================
        //
        // If a maintenance worker moves halls, remove them from
        // all unresolved complaints they are currently assigned to.
        //
        // We DO NOT change the complaint status.
        //
        // Assigned   -> Assigned
        // InProgress -> InProgress
        //
        // This means the work itself is not forgotten.
        // Only the responsible technician is removed.
        //
        // ========================================================

        if (
            hallChanged &&
            user.Role == UserRole.Maintenance
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
                // Remove the old technician.

                complaint.AssignedToId = null;

                // IMPORTANT:
                //
                // Do NOT do this:
                //
                // complaint.Status =
                //     ComplaintStatus.Submitted;
                //
                // We want the current progress to remain intact.
            }
        }


        // ========================================================
        // UPDATE USER INFORMATION
        // ========================================================

        user.FullName =
            fullName;

        user.Email =
            email;

        user.Role =
            role;

        user.Hall =
            hall;


        // ========================================================
        // UPDATE SPECIALIZATION
        // ========================================================
        //
        // Only Maintenance users should have a specialization.
        //
        // ========================================================

        user.MaintenanceSpecialization =
            role == UserRole.Maintenance
                ? maintenanceSpecialization
                : null;


        // ========================================================
        // UPDATE ACTIVE STATUS
        // ========================================================

        user.IsActive =
            isActive;


        // ========================================================
        // SAVE EVERYTHING
        // ========================================================

        await _context.SaveChangesAsync();

        return true;
    }


    // ============================================================
    // ACTIVATE / DEACTIVATE
    // ============================================================

    public async Task<bool> SetActiveAsync(
        int id,
        bool isActive)
    {
        var user =
            await _context.Users
                .FindAsync(id);

        if (user == null)
        {
            return false;
        }


        user.IsActive =
            isActive;


        await _context.SaveChangesAsync();

        return true;
    }


    // ============================================================
    // DELETE USER
    // ============================================================

    public async Task<bool> DeleteAsync(
        int id)
    {
        var user =
            await _context.Users
                .FindAsync(id);

        if (user == null)
        {
            return false;
        }


        // --------------------------------------------------------
        // Never delete an administrator.
        // --------------------------------------------------------

        if (user.Role == UserRole.Admin)
        {
            return false;
        }


        // --------------------------------------------------------
        // Do not delete users who have complaint assignments.
        // --------------------------------------------------------

        var hasAssignedComplaints =
            await _context.Complaints
                .AnyAsync(
                    c => c.AssignedToId == id
                );

        if (hasAssignedComplaints)
        {
            return false;
        }


        // --------------------------------------------------------
        // Do not delete users who have audit history.
        // --------------------------------------------------------

        var hasComplaintUpdates =
            await _context.ComplaintUpdates
                .AnyAsync(
                    u => u.UpdatedById == id
                );

        if (hasComplaintUpdates)
        {
            return false;
        }


        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }


    // ============================================================
    // VERIFY PASSWORD
    // ============================================================

    public bool VerifyPassword(
        ApplicationUser user,
        string password)
    {
        return _hasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password
        ) != PasswordVerificationResult.Failed;
    }
}