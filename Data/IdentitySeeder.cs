using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;

namespace KnustHallMaintenance.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole<int>> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        var roles = new[]
        {
            UserRole.HallStaff.ToString(),
            UserRole.Maintenance.ToString(),
            UserRole.Admin.ToString()
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<int>(role)
                );
            }
        }

        var admin =
            await userManager.FindByEmailAsync(
                "admin@knust.edu.gh"
            );

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin@knust.edu.gh",
                Email = "admin@knust.edu.gh",
                FullName = "System Administrator",
                IsActive = true
            };

            var result =
                await userManager.CreateAsync(
                    admin,
                    "Admin123!"
                );

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        e => e.Description
                    )
                );

                throw new InvalidOperationException(
                    errors
                );
            }
        }

        if (
            !await userManager.IsInRoleAsync(
                admin,
                UserRole.Admin.ToString()
            )
        )
        {
            var result =
                await userManager.AddToRoleAsync(
                    admin,
                    UserRole.Admin.ToString()
                );

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        e => e.Description
                    )
                );

                throw new InvalidOperationException(
                    errors
                );
            }
        }
    }
}