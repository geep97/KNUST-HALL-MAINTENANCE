using System.Security.Claims;
using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KnustHallMaintenance.Data;

public class ApplicationUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole<int>>
{
    public ApplicationUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(
        ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        if (!string.IsNullOrWhiteSpace(user.Hall))
        {
            identity.AddClaim(
                new Claim("Hall", user.Hall)
            );
        }

        return identity;
    }
}