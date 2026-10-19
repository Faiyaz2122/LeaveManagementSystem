using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Services;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Controllers
{
    public class LeaveRequestController : Controller
    {
        private readonly LeaveDbContext _context;
        private readonly LeaveRequestService _leaveService;

        public LeaveRequestController(LeaveDbContext context, LeaveRequestService leaveService)
        {
            _context = context;
            _leaveService = leaveService;
        }

        // GET: LeaveRequest (Master Management Matrix Dashboard view link)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var history = await _leaveService.GetAllRequestsAsync();
            return View(history);
        }

        // =========================================================
        // STEP 8: EMPLOYEE SPECIFIC LEAVE HISTORY LOG VIEWER
        // =========================================================
        // GET: LeaveRequest/HistoryLog?employeeId=2
        [HttpGet]
        public async Task<IActionResult> HistoryLog(int? employeeId)
        {
            if (employeeId == null)
            {
                // If no specific employee profile is queried, default fallback display parameters to first active index profile found
                var firstEmp = await _context.Employees.FirstOrDefaultAsync();
                if (firstEmp == null) return NotFound("No employee data profiles present to construct history logs summary pipelines.");
                employeeId = firstEmp.EmployeeId;
            }

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return NotFound();

            ViewBag.EmployeeName = employee.Name;
            ViewBag.EmployeeList = new SelectList(_context.Employees, "EmployeeId", "Name", employeeId);

            var personalHistory = await _leaveService.GetEmployeeLeaveHistoryAsync(employeeId.Value);
            return View(personalHistory);
        }

        // GET: LeaveRequest/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(_context.Employees, "EmployeeId", "Name");
            ViewBag.LeaveTypeId = new SelectList(_context.LeaveTypes, "LeaveTypeId", "LeaveName");
            return View();
        }

        // POST: LeaveRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequest request)
        {
            ViewBag.EmployeeId = new SelectList(_context.Employees, "EmployeeId", "Name", request.EmployeeId);
            ViewBag.LeaveTypeId = new SelectList(_context.LeaveTypes, "LeaveTypeId", "LeaveName", request.LeaveTypeId);

            var targetEmployee = await _context.Employees.FindAsync(request.EmployeeId);
            if (targetEmployee == null || !targetEmployee.IsActive)
            {
                ModelState.AddModelError("", "Selected employee profile is inactive. Leave cannot be requested.");
                return View(request);
            }

            // CONSUMING STEP 9 SERVICE LAYERS: Date validation check
            if (!_leaveService.IsDateRangeValid(request.FromDate, request.ToDate))
            {
                ModelState.AddModelError("FromDate", "The start date cannot be later than the end date.");
                return View(request);
            }

            // CONSUMING STEP 9 SERVICE LAYERS: Dynamic query overlap prevention check
            if (await _leaveService.HasOverlappingLeaveAsync(request.EmployeeId, request.FromDate, request.ToDate))
            {
                ModelState.AddModelError("", "Application dates overlap with an existing scheduled leave period.");
                return View(request);
            }

            request.NumberOfDays = (request.ToDate.Date - request.FromDate.Date).Days + 1;
            request.Status = "Pending";

            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        // GET: LeaveRequest/ProcessApproval/5
        [HttpGet]
        public async Task<IActionResult> ProcessApproval(int id)
        {
            var request = await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(m => m.LeaveRequestId == id);

            if (request == null) return NotFound();
            return View(request);
        }

        // POST: LeaveRequest/SaveApprovalDecision
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveApprovalDecision(int LeaveRequestId, string approvalAction, string? Comments)
        {
            var request = await _context.LeaveRequests.FindAsync(LeaveRequestId);
            if (request == null) return NotFound();

            if (approvalAction == "Approved") request.Status = "Approved";
            else if (approvalAction == "Rejected") request.Status = "Rejected";

            request.Comments = !string.IsNullOrWhiteSpace(Comments) ? Comments.Trim() : null;

            _context.Update(request);
            await _context.SaveChangesAsync();

            TempData["WorkflowSuccess"] = $"Case record successfully resolved as status code: '{request.Status}'.";
            return RedirectToAction(nameof(Index));
        }
    }
}
