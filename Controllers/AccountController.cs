using KnustHallMaintenance.Models;
using KnustHallMaintenance.Models.ViewModels;
using KnustHallMaintenance.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KnustHallMaintenance.Controllers;

public class AccountController : Controller
{
    private readonly IStudentService _studentService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        IStudentService studentService,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _studentService = studentService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (LooksLikeStudentId(model.EmailOrStudentId))
        {
            var student =
                await _studentService
                    .GetByStudentIdAsync(
                        model.EmailOrStudentId.Trim()
                    );

            if (
                student == null ||
                !_studentService.VerifyPassword(
                    student,
                    model.Password
                )
            )
            {
                model.ErrorMessage =
                    "Invalid student ID or password.";

                return View(model);
            }

            var studentClaims =
                new List<Claim>
                {
                    new(
                        ClaimTypes.NameIdentifier,
                        student.Id.ToString()
                    ),

                    new(
                        ClaimTypes.Name,
                        student.FullName
                    ),

                    new(
                        ClaimTypes.Email,
                        student.Email
                    ),

                    new(
                        ClaimTypes.Role,
                        "Student"
                    ),

                    new(
                        "StudentId",
                        student.StudentId
                    ),

                    new(
                        "Hall",
                        student.Hall
                    ),

                    new(
                        "RoomNumber",
                        student.RoomNumber
                    ),

                    new(
                        "Programme",
                        student.Programme
                    )
                };

            var studentIdentity =
                new ClaimsIdentity(
                    studentClaims,
                    IdentityConstants.ApplicationScheme
                );

            var studentPrincipal =
                new ClaimsPrincipal(
                    studentIdentity
                );

            await HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                studentPrincipal
            );

            return RedirectToAction(
                "MyComplaints",
                "Complaints"
            );
        }

        var user =
            await _userManager
                .FindByEmailAsync(
                    model.EmailOrStudentId.Trim()
                );

        if (user == null)
        {
            model.ErrorMessage =
                "Invalid email or password.";

            return View(model);
        }

        if (!user.IsActive)
        {
            model.ErrorMessage =
                "This account has been deactivated. Please contact the administrator.";

            return View(model);
        }

        var result =
            await _signInManager
                .PasswordSignInAsync(
                    user,
                    model.Password,
                    isPersistent: false,
                    lockoutOnFailure: false
                );

        if (result.Succeeded)
        {
            var roles =
                await _userManager
                    .GetRolesAsync(user);

            var role =
                roles.FirstOrDefault();

            return RedirectAfterLogin(role);
        }

        if (result.IsLockedOut)
        {
            model.ErrorMessage =
                "This account is temporarily locked.";

            return View(model);
        }

        model.ErrorMessage =
            "Invalid email or password.";

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(
            "Login",
            "Account"
        );
    }

    private IActionResult RedirectAfterLogin(
        string? role)
    {
        if (
            role ==
            UserRole.HallStaff.ToString()
        )
        {
            return RedirectToAction(
                "Index",
                "Staff"
            );
        }

        if (
            role ==
            UserRole.Maintenance.ToString()
        )
        {
            return RedirectToAction(
                "Index",
                "Staff"
            );
        }

        if (
            role ==
            UserRole.Admin.ToString()
        )
        {
            return RedirectToAction(
                "Index",
                "Admin"
            );
        }

        return RedirectToAction(
            "Login",
            "Account"
        );
    }

    private static bool LooksLikeStudentId(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.Length == 8 &&
               value.All(char.IsDigit);
    }
}