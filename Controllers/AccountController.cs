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

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }


    //login-get

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectAfterLogin();

        return View(new LoginViewModel());
    }


    //login-post

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);


        var user =
            await _userService.GetByEmailAsync(model.Email);


        if (
            user == null ||
            !_userService.VerifyPassword(
                user,
                model.Password
            )
        )
        {
            model.ErrorMessage =
                "Invalid email or password.";

            return View(model);
        }


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
            )
        };


        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );


        var principal =
            new ClaimsPrincipal(identity);


        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );


        return RedirectAfterLogin(user.Role);
    }


    //logout

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction(nameof(Login));
    }

    //rbac
    private IActionResult RedirectAfterLogin(
        UserRole? role = null
    )
    {
        var roleClaim =
            role?.ToString()
            ?? User.FindFirstValue(ClaimTypes.Role);


        //student

        if (
            roleClaim ==
            UserRole.Student.ToString()
        )
        {
            return RedirectToAction(
                "MyComplaints",
                "Complaints"
            );
        }


        //staff

        if (
            roleClaim ==
            UserRole.Staff.ToString()
        )
        {
            return RedirectToAction(
                "Index",
                "Staff"
            );
        }


        //maintainance

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


        //admin

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


        
        return RedirectToAction(
            "Login",
            "Account"
        );
    }
}