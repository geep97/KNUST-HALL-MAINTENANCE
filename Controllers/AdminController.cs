using KnustHallMaintenance.Models;
using KnustHallMaintenance.Models.ViewModels;
using KnustHallMaintenance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KnustHallMaintenance.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminController : Controller
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    // ============================================================
    // ADMIN DASHBOARD
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();

        return View(users);
    }


    // ============================================================
    // CREATE USER - GET
    // ============================================================

    [HttpGet]
    public IActionResult CreateUser()
    {
        var model = new AdminCreateUserViewModel
        {
            Role = UserRole.Student
        };

        return View(model);
    }


    // ============================================================
    // CREATE USER - POST
    // ============================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(
        AdminCreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }


        // --------------------------------------------------------
        // Check whether email already exists
        // --------------------------------------------------------

        var existingUser =
            await _userService.GetByEmailAsync(model.Email);

        if (existingUser != null)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "A user with this email already exists.");

            return View(model);
        }


        // --------------------------------------------------------
        // Create the user
        // --------------------------------------------------------

        await _userService.CreateAsync(
            model.FullName,
            model.Email,
            model.Password,
            model.Role,
            model.Hall,
            model.RoomNumber
        );


        // --------------------------------------------------------
        // Success message
        // --------------------------------------------------------

        TempData["Success"] =
            $"{model.FullName}'s account was created successfully.";


        return RedirectToAction(nameof(Index));
    }
}