using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;

namespace KnustHallMaintenance.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager)
    {
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
                var errors =
                    string.Join(
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
                var errors =
                    string.Join(
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