using System.Security.Claims;
using KnustHallMaintenance.Models;
using KnustHallMaintenance.Models.ViewModels;
using KnustHallMaintenance.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace KnustHallMaintenance.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IStudentService _studentService;

    public AccountController(
        IUserService userService,
        IStudentService studentService)
    {
        _userService = userService;
        _studentService = studentService;
    }


    // =========================================================
    // LOGIN - GET
    // =========================================================

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }


    // =========================================================
    // LOGIN - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }


        // =====================================================
        // STUDENT LOGIN
        // =====================================================

        if (LooksLikeStudentId(model.EmailOrStudentId))
        {
            var student =
                await _studentService.GetByStudentIdAsync(
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


            // =================================================
            // STUDENT CLAIMS
            // =================================================

            var studentClaims = new List<Claim>
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
                    CookieAuthenticationDefaults.AuthenticationScheme
                );


            var studentPrincipal =
                new ClaimsPrincipal(
                    studentIdentity
                );


            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                studentPrincipal
            );


            return RedirectToAction(
                "MyComplaints",
                "Complaints"
            );
        }


        // =====================================================
        // PORTAL USER LOGIN
        // Admin / Hall Staff / Maintenance
        // =====================================================

        var user =
    await _userService.GetByEmailAsync(
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


        if (!_userService.VerifyPassword(
                user,
                model.Password))
        {
            model.ErrorMessage =
                "Invalid email or password.";

            return View(model);
        }


        // =====================================================
        // PORTAL USER CLAIMS
        // =====================================================

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),

            new(
                ClaimTypes.Name,
                user.FullName
            ),

            new(
                ClaimTypes.Email,
                user.Email
            ),

            new(
                ClaimTypes.Role,
                user.Role.ToString()
            ),

            // =================================================
            // HALL CLAIM
            // =================================================
            //
            // Hall Staff and Maintenance users have a hall.
            //
            // Admin users normally have no hall, so an empty
            // claim is stored for them.
            //
            new(
                "Hall",
                user.Hall ?? ""
            )
        };


        var identity =
            new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );


        var principal =
            new ClaimsPrincipal(identity);


        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );


        return RedirectAfterLogin(
            user.Role
        );
    }


    // =========================================================
    // LOGOUT
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );


        // Explicitly remove the authentication cookie.
        Response.Cookies.Delete(
            ".AspNetCore.Cookies"
        );


        return RedirectToAction(
            "Login",
            "Account"
        );
    }


    // =========================================================
    // LOGIN REDIRECTION
    // =========================================================

    private IActionResult RedirectAfterLogin(
        UserRole? role = null)
    {
        var roleClaim =
            role?.ToString()
            ?? User.FindFirstValue(
                ClaimTypes.Role
            );


        // -----------------------------------------------------
        // Student
        // -----------------------------------------------------

        if (roleClaim == "Student")
        {
            return RedirectToAction(
                "MyComplaints",
                "Complaints"
            );
        }


        // -----------------------------------------------------
        // Hall Staff
        // -----------------------------------------------------

        if (
            roleClaim ==
            UserRole.HallStaff.ToString()
        )
        {
            return RedirectToAction(
                "Index",
                "Staff"
            );
        }


        // -----------------------------------------------------
        // Maintenance
        // -----------------------------------------------------

        if (
            roleClaim ==
            UserRole.Maintenance.ToString()
        )
        {
            return RedirectToAction(
                "Index",
                "Staff"
            );
        }


        // -----------------------------------------------------
        // Admin
        // -----------------------------------------------------

        if (
            roleClaim ==
            UserRole.Admin.ToString()
        )
        {
            return RedirectToAction(
                "Index",
                "Admin"
            );
        }


        // -----------------------------------------------------
        // Unknown role
        // -----------------------------------------------------

        return RedirectToAction(
            "Login",
            "Account"
        );
    }


    // =========================================================
    // STUDENT ID DETECTION
    // =========================================================

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