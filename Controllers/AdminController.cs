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


    // =========================================================
    // AVAILABLE HALLS
    // =========================================================

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


    // =========================================================
    // ADMIN DASHBOARD
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users =
            await _userService.GetAllAsync();

        return View(users);
    }


    // =========================================================
    // CREATE USER - GET
    // =========================================================

    [HttpGet]
    public IActionResult CreateUser()
    {
        ViewBag.Halls = Halls;

        return View(
            new AdminCreateUserViewModel()
        );
    }


    // =========================================================
    // CREATE USER - POST
    // =========================================================

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


    // =========================================================
    // EDIT USER - GET
    // =========================================================

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


        var model = new AdminEditUserViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Hall = user.Hall,
            MaintenanceSpecialization =
                user.MaintenanceSpecialization,
            IsActive = user.IsActive
        };


        return View(model);
    }


    // =========================================================
    // EDIT USER - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(
        AdminEditUserViewModel model)
    {
        ViewBag.Halls = Halls;


        var existingUser =
            await _userService.GetByIdAsync(model.Id);

        if (existingUser == null)
        {
            return NotFound();
        }


        // -----------------------------------------------------
        // Prevent changing an Admin into another role.
        // Prevent editing Admin role into something else.
        // -----------------------------------------------------

        if (
            existingUser.Role == UserRole.Admin &&
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


        // -----------------------------------------------------
        // Duplicate email check
        // -----------------------------------------------------

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


    // =========================================================
    // ACTIVATE / DEACTIVATE
    // =========================================================

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


        if (user.Role == UserRole.Admin)
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


    // =========================================================
    // DELETE USER
    // =========================================================

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


        if (user.Role == UserRole.Admin)
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


    // =========================================================
    // VALIDATION
    // =========================================================

    private void ValidateUserFields(
        UserRole role,
        string? hall,
        MaintenanceSpecialization? specialization)
    {
        // -----------------------------------------------------
        // Hall Staff + Maintenance require a hall
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // Maintenance requires specialization
        // -----------------------------------------------------

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
}