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


        // -----------------------------------------------------
        // Hall is required for Hall Staff and Maintenance
        // -----------------------------------------------------

        if (
            model.Role == UserRole.HallStaff ||
            model.Role == UserRole.Maintenance
        )
        {
            if (string.IsNullOrWhiteSpace(model.Hall))
            {
                ModelState.AddModelError(
                    nameof(model.Hall),
                    "Please select a hall."
                );
            }
            else if (!Halls.Contains(model.Hall))
            {
                ModelState.AddModelError(
                    nameof(model.Hall),
                    "Please select a valid hall."
                );
            }
        }


        // -----------------------------------------------------
        // Admin should not have a hall
        // -----------------------------------------------------

        if (model.Role == UserRole.Admin)
        {
            model.Hall = null;
        }


        // -----------------------------------------------------
        // Check validation
        // -----------------------------------------------------

        if (!ModelState.IsValid)
        {
            return View(model);
        }


        // -----------------------------------------------------
        // Prevent duplicate email
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // Create user
        // -----------------------------------------------------

        await _userService.CreateAsync(
            model.FullName.Trim(),
            model.Email.Trim(),
            model.Password,
            model.Role,
            model.Hall
        );


        // -----------------------------------------------------
        // Success
        // -----------------------------------------------------

        TempData["Success"] =
            $"{model.FullName}'s account was created successfully.";

        return RedirectToAction(
            nameof(Index)
        );
    }
}