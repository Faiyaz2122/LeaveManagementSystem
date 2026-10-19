using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Controllers
{
    public class LeaveTypeController : Controller
    {
        private readonly LeaveDbContext _context;

        // Constructor: Connects our controller to the SQL Server database
        public LeaveTypeController(LeaveDbContext context)
        {
            _context = context;
        }

        // Action 1: Displays the table list of leave categories
        public async Task<IActionResult> Index()
        {
            var leaveTypes = await _context.LeaveTypes.ToListAsync();
            return View(leaveTypes);
        }

        // Action 2: Loads the blank 'Add New Leave Type' page form
        public IActionResult Create()
        {
            return View();
        }

        // Action 3: Receives form data from browser and saves it to SQL Server
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveType leaveType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Go back to the table list after saving
            }
            return View(leaveType);
        }
    }
}
