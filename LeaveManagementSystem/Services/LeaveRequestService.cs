using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Services
{
    public class LeaveRequestService
    {
        private readonly LeaveDbContext _context;

        public LeaveRequestService(LeaveDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // STEP 9: BUSINESS RULES VALIDATION MATRICES
        // ==========================================

        // Rule A: Chronological range sequence boundaries validation check
        public bool IsDateRangeValid(DateTime fromDate, DateTime toDate)
        {
            return fromDate.Date <= toDate.Date;
        }

        // Rule B: Cross-concurrency database overlap validation prevention check
        public async Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime fromDate, DateTime toDate, int? currentRequestId = null)
        {
            return await _context.LeaveRequests
                .AnyAsync(r => r.EmployeeId == employeeId &&
                               r.Status != "Rejected" &&
                               r.LeaveRequestId != currentRequestId && // Ignore self check if updating an item
                               ((fromDate.Date >= r.FromDate.Date && fromDate.Date <= r.ToDate.Date) ||
                                (toDate.Date >= r.FromDate.Date && toDate.Date <= r.ToDate.Date) ||
                                (fromDate.Date <= r.FromDate.Date && toDate.Date >= r.ToDate.Date)));
        }

        // ========================================================
        // STEP 8: EMPLOYEE HISTORY & GENERAL SEARCH QUERY ENGINES
        // ========================================================

        // Fetch all history logs filtered by an explicit employee primary key sequence index identifier
        public async Task<List<LeaveRequest>> GetEmployeeLeaveHistoryAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .Include(r => r.LeaveType)
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.FromDate)
                .ToListAsync();
        }

        // Fetch master rows logs summary with comprehensive tracking indices references loaded
        public async Task<List<LeaveRequest>> GetAllRequestsAsync()
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .OrderByDescending(r => r.FromDate)
                .ToListAsync();
        }
    }
}
