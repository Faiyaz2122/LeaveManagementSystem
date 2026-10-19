using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly LeaveDbContext _context;

        public AccountController(LeaveDbContext context)
        {
            _context = context;
        }

        // GET: Account/Signup
        [HttpGet]
        public IActionResult Signup()
        {
            ViewBag.DepartmentId = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            return View();
        }

        // POST: Account/Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(Employee model, string ConfirmPassword)
        {
            ViewBag.DepartmentId = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", model.DepartmentId);

            if (model.Password != ConfirmPassword)
            {
                ModelState.AddModelError("", "The password and confirmation password do not match.");
                return View(model);
            }

            bool emailExists = await _context.Employees.AnyAsync(e => e.Email.ToLower() == model.Email.ToLower());
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email address is already registered.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                model.IsActive = true;

                // Assign standard worker default role by query lookup reference mapping
                var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Employee");
                if (defaultRole != null)
                {
                    model.RoleId = defaultRole.RoleId;
                }

                _context.Employees.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Account created successfully! Please login below.";
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.Session.Clear(); // Flush memory tracking variables securely
            return View();
        }

        // POST: Account/Login (UPDATED FOR USERNAME AUTHENTICATION)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Please fill in all credential fields.");
                return View();
            }

            // Matches the database row based on Username instead of Email
            var user = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.SystemRole)
                .FirstOrDefaultAsync(e => e.Username != null && e.Username.ToLower() == username.ToLower() && e.Password == password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login credentials match sequence.");
                return View();
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "This account profile context is locked or deactivated.");
                return View();
            }

            // Establish active tracking memory session variables across dashboards safely
            HttpContext.Session.SetInt32("UserId", user.EmployeeId);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.SystemRole?.RoleName ?? "Employee");

            return RedirectToAction("Index", "Home");
        }


        // GET: Account/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Terminate tracking session references completely
            return RedirectToAction(nameof(Login));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordAdmin(int employeeId, string newPassword)
        {
            // Restriction Gate check: If the active user isn't logged in explicitly as a Manager
            if (HttpContext.Session.GetString("UserRole") != "Manager")
            {
                // Block processing instantly and force push them away
                TempData["ErrorMessage"] = "Access Denied: You do not possess structural manager clearances to use this gateway.";
                return RedirectToAction("Index", "Home");
            }

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return NotFound();

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "Password override failed: Reset password must be at least 6 characters long.";
                return RedirectToAction("Index", "Employee");
            }

            employee.Password = newPassword.Trim();
            _context.Update(employee);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully updated temporary keys for: '{employee.Name}'.";
            return RedirectToAction("Index", "Employee");
        }
    }
}
