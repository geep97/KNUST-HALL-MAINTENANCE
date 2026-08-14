using Microsoft.AspNetCore.Mvc;

namespace KnustHallMaintenance.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Login", "Account");
    }

    public IActionResult Error()
    {
        return View();
    }
}
