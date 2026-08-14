using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;

namespace KnustHallMaintenance.Data;

public static class DbInitializer
{
    public static void Seed(
        ApplicationDbContext context,
        IPasswordHasher<ApplicationUser> hasher)
    {
        // ============================================================
        // ADMIN
        // ============================================================

        if (!context.Users.Any(u => u.Role == UserRole.Admin))
        {
            var admin = new ApplicationUser
            {
                FullName = "System Administrator",
                Email = "admin@knust.edu.gh",
                Role = UserRole.Admin
            };

            admin.PasswordHash =
                hasher.HashPassword(admin, "Admin123!");

            context.Users.Add(admin);
        }


        // ============================================================
        // HALL STAFF
        // ============================================================

        if (!context.Users.Any(u => u.Role == UserRole.HallStaff))
        {
            var staff = new ApplicationUser
            {
                FullName = "Hall Assistant - Unity Hall",
                Email = "staff@knust.edu.gh",
                Role = UserRole.HallStaff,
                Hall = "Unity Hall"
            };

            staff.PasswordHash =
                hasher.HashPassword(staff, "Password123!");

            context.Users.Add(staff);
        }


        // ============================================================
        // MAINTENANCE PERSONNEL
        // ============================================================

        if (!context.Users.Any(u => u.Role == UserRole.Maintenance))
        {
            var technician = new ApplicationUser
            {
                FullName = "John Boateng (Electrician)",
                Email = "maintenance@knust.edu.gh",
                Role = UserRole.Maintenance
            };

            technician.PasswordHash =
                hasher.HashPassword(technician, "Password123!");

            context.Users.Add(technician);
        }


        // Save Admin / Staff / Maintenance
        context.SaveChanges();
    }
}