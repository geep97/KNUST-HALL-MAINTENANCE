using KnustHallMaintenance.Models;
using KnustHallMaintenance.Models.ViewModels;
using KnustHallMaintenance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KnustHallMaintenance.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUserService _userService;

    public AdminController(
        IUserService userService)
    {
        _userService = userService;
    }

    private static readonly List<string> Halls =
        new()
        {
            "Unity Hall",
            "Independence Hall",
            "Republic Hall",
            "Queen Elizabeth II Hall",
            "Africa Hall",
            "University Hall"
        };

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users =
            await _userService.GetAllAsync();

        var model =
            new List<AdminUserViewModel>();

        foreach (var user in users)
        {
            var role =
                UserRole.HallStaff;

            if (
                await _userService.IsInRoleAsync(
                    user,
                    UserRole.Admin
                )
            )
            {
                role = UserRole.Admin;
            }
            else if (
                await _userService.IsInRoleAsync(
                    user,
                    UserRole.Maintenance
                )
            )
            {
                role = UserRole.Maintenance;
            }
            else if (
                await _userService.IsInRoleAsync(
                    user,
                    UserRole.HallStaff
                )
            )
            {
                role = UserRole.HallStaff;
            }

            model.Add(
                new AdminUserViewModel
                {
                    User = user,
                    Role = role
                }
            );
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult CreateUser()
    {
        ViewBag.Halls = Halls;

        return View(
            new AdminCreateUserViewModel()
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(
        AdminCreateUserViewModel model)
    {
        ViewBag.Halls = Halls;

        ValidateUserFields(
            model.Role,
            model.Hall,
            model.MaintenanceSpecialization
        );

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existingUser =
            await _userService.GetByEmailAsync(
                model.Email.Trim()
            );

        if (existingUser != null)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "An account with this email already exists."
            );

            return View(model);
        }

        await _userService.CreateAsync(
            model.FullName.Trim(),
            model.Email.Trim(),
            model.Password,
            model.Role,
            model.Hall,
            model.MaintenanceSpecialization
        );

        TempData["Success"] =
            $"{model.FullName}'s account was created successfully.";

        return RedirectToAction(
            nameof(Index)
        );
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(
        int id)
    {
        var user =
            await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        ViewBag.Halls = Halls;

        var roles =
            await GetUserRolesAsync(user);

        var role =
            roles.FirstOrDefault();

        var model =
            new AdminEditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email?? string.Empty,
                Role = ParseRole(role),
                Hall = user.Hall,
                MaintenanceSpecialization =
                    user.MaintenanceSpecialization,
                IsActive = user.IsActive
            };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(
        AdminEditUserViewModel model)
    {
        ViewBag.Halls = Halls;

        var existingUser =
            await _userService.GetByIdAsync(
                model.Id
            );

        if (existingUser == null)
        {
            return NotFound();
        }

        var existingUserIsAdmin =
            await _userService.IsInRoleAsync(
                existingUser,
                UserRole.Admin
            );

        if (
            existingUserIsAdmin &&
            model.Role != UserRole.Admin
        )
        {
            ModelState.AddModelError(
                nameof(model.Role),
                "Administrator accounts cannot be changed to another role."
            );
        }

        ValidateUserFields(
            model.Role,
            model.Hall,
            model.MaintenanceSpecialization
        );

        var emailOwner =
            await _userService.GetByEmailAsync(
                model.Email.Trim()
            );

        if (
            emailOwner != null &&
            emailOwner.Id != model.Id
        )
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "Another account already uses this email."
            );
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _userService.UpdateAsync(
            model.Id,
            model.FullName.Trim(),
            model.Email.Trim(),
            model.Role,
            model.Hall,
            model.MaintenanceSpecialization,
            model.IsActive
        );

        TempData["Success"] =
            $"{model.FullName}'s account was updated successfully.";

        return RedirectToAction(
            nameof(Index)
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(
        int id)
    {
        var user =
            await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var isAdmin =
            await _userService.IsInRoleAsync(
                user,
                UserRole.Admin
            );

        if (isAdmin)
        {
            TempData["Error"] =
                "Administrator accounts cannot be deactivated.";

            return RedirectToAction(
                nameof(Index)
            );
        }

        var newStatus =
            !user.IsActive;

        await _userService.SetActiveAsync(
            id,
            newStatus
        );

        TempData["Success"] =
            newStatus
                ? $"{user.FullName} has been activated."
                : $"{user.FullName} has been deactivated.";

        return RedirectToAction(
            nameof(Index)
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(
        int id)
    {
        var user =
            await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var isAdmin =
            await _userService.IsInRoleAsync(
                user,
                UserRole.Admin
            );

        if (isAdmin)
        {
            TempData["Error"] =
                "Administrator accounts cannot be deleted.";

            return RedirectToAction(
                nameof(Index)
            );
        }

        var deleted =
            await _userService.DeleteAsync(id);

        if (!deleted)
        {
            TempData["Error"] =
                $"{user.FullName} cannot be deleted because the account has existing complaint or audit history. Deactivate the account instead.";
        }
        else
        {
            TempData["Success"] =
                $"{user.FullName}'s account was deleted.";
        }

        return RedirectToAction(
            nameof(Index)
        );
    }

    private void ValidateUserFields(
        UserRole role,
        string? hall,
        MaintenanceSpecialization? specialization)
    {
        if (
            role == UserRole.HallStaff ||
            role == UserRole.Maintenance
        )
        {
            if (string.IsNullOrWhiteSpace(hall))
            {
                ModelState.AddModelError(
                    nameof(AdminCreateUserViewModel.Hall),
                    "Please select a hall."
                );
            }
            else if (!Halls.Contains(hall))
            {
                ModelState.AddModelError(
                    nameof(AdminCreateUserViewModel.Hall),
                    "Please select a valid hall."
                );
            }
        }

        if (role == UserRole.Maintenance)
        {
            if (!specialization.HasValue)
            {
                ModelState.AddModelError(
                    nameof(AdminCreateUserViewModel.MaintenanceSpecialization),
                    "Please select a maintenance specialization."
                );
            }
        }
    }

    private async Task<IList<string>> GetUserRolesAsync(
        ApplicationUser user)
    {
        var roles =
            new List<string>();

        foreach (
            var role in
            new[]
            {
                UserRole.Admin,
                UserRole.HallStaff,
                UserRole.Maintenance
            })
        {
            if (
                await _userService.IsInRoleAsync(
                    user,
                    role
                )
            )
            {
                roles.Add(
                    role.ToString()
                );
            }
        }

        return roles;
    }

    private static UserRole ParseRole(
        string? role)
    {
        if (
            Enum.TryParse<UserRole>(
                role,
                out var parsedRole
            )
        )
        {
            return parsedRole;
        }

        return UserRole.HallStaff;
    }
}