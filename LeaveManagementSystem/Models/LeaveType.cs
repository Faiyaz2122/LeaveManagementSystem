using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    public class LeaveType
    {
        public int LeaveTypeId { get; set; }

        [Required(ErrorMessage = "Leave type name is required.")]
        [Display(Name = "Leave Type Name")]
        public string LeaveName { get; set; } = null!;

        [Range(1, 365, ErrorMessage = "Maximum days must be 1 or greater.")]
        public int? MaxDays { get; set; }

        // Navigation
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
