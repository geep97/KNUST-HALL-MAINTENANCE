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
                hasher.HashPassword(
                    admin,
                    "Admin123!"
                );

            context.Users.Add(admin);
        }


        // ============================================================
        // SAVE
        // ============================================================

        context.SaveChanges();
    }
}