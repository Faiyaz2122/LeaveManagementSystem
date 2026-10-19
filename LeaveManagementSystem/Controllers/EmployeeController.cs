using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly LeaveDbContext _context;

        public EmployeeController(LeaveDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET: Employee (Directory Matrix Grid)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Security Gate: Restrict access to Manager profiles explicitly
            if (HttpContext.Session.GetString("UserRole") != "Manager")
            {
                TempData["ErrorMessage"] = "Security Violation: Access denied to administrative directory modules.";
                return RedirectToAction("Index", "Home");
            }

            // Performance Optimized: Eager load relations with AsNoTracking to minimize latency limits
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.SystemRole)
                .AsNoTracking()
                .ToListAsync();

            return View(employees);
        }

        // ==========================================
        // GET: Employee/Details/5
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            if (HttpContext.Session.GetString("UserRole") != "Manager")
            {
                TempData["ErrorMessage"] = "Access Denied: Administrative authorization clearance required.";
                return RedirectToAction("Index", "Home");
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.SystemRole)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // ==========================================
        // GET: Employee/Create
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Manager")
            {
                TempData["ErrorMessage"] = "Access Denied: Administrative authorization clearance required.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.DepartmentId = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // ==========================================
        // POST: Employee/Create
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (HttpContext.Session.GetString("UserRole") != "Manager")
            {
                TempData["ErrorMessage"] = "Access Denied: Security compliance block active.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Employee profile for '{employee.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DepartmentId = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", employee.RoleId);
            return View(employee);
        }

        // ==========================================
        // GET: Employee/Edit/5
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            if (HttpContext.Session.GetString("UserRole") != "Manager")
            {
                TempData["ErrorMessage"] = "Access Denied: Administrative authorization clearance required.";
                return RedirectToAction("Index", "Home");
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            ViewBag.DepartmentId = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", employee.RoleId);
            return View(employee);
        }

        // ==========================================
        // POST: Employee/Edit/5
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId) return NotFound();

            if (HttpContext.Session.GetString("UserRole") != "Manager")
            {
                TempData["ErrorMessage"] = "Access Denied: Security compliance block active.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Employee profile modifications saved successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DepartmentId = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", employee.RoleId);
            return View(employee);
        }

        // ==========================================
        // POST: Employee/UpdateInlineRole (Asynchronous Inline Dashboard Adjustments)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInlineRole(int selectedEmployeeId, int? newRoleId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Manager")
            {
                TempData["ErrorMessage"] = "Unauthorized: Manager permissions are required to perform role assignment modifications.";
                return RedirectToAction(nameof(Index));
            }

            var employee = await _context.Employees.FindAsync(selectedEmployeeId);
            if (employee == null) return NotFound();

            // Set the new numeric lookup Key parameter relation directly
            employee.RoleId = newRoleId;

            _context.Update(employee);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Access privileges updated successfully for employee: '{employee.Name}'.";
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
