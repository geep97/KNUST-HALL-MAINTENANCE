using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;

namespace KnustHallMaintenance.Data;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole<int>> roleManager)
    {
        var roles =
            new[]
            {
                new IdentityRole<int>
                {
                    Id = (int)UserRole.HallStaff,
                    Name = UserRole.HallStaff.ToString(),
                    NormalizedName =
                        UserRole.HallStaff.ToString().ToUpperInvariant()
                },

                new IdentityRole<int>
                {
                    Id = (int)UserRole.Maintenance,
                    Name = UserRole.Maintenance.ToString(),
                    NormalizedName =
                        UserRole.Maintenance.ToString().ToUpperInvariant()
                },

                new IdentityRole<int>
                {
                    Id = (int)UserRole.Admin,
                    Name = UserRole.Admin.ToString(),
                    NormalizedName =
                        UserRole.Admin.ToString().ToUpperInvariant()
                }
            };

        foreach (var role in roles)
        {
            var existingRole =
                await roleManager.FindByIdAsync(
                    role.Id.ToString()
                );

            if (existingRole == null)
            {
                await roleManager.CreateAsync(role);
            }
        }
    }
}