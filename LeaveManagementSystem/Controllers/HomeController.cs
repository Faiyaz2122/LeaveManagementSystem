using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LeaveManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                // Instantly force redirect back to authentication login panel workspace registers
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
