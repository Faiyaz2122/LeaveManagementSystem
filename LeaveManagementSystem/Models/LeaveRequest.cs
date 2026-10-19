using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagementSystem.Models
{
    public class LeaveRequest
    {
        [Key]
        public int LeaveRequestId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }

        [ForeignKey("LeaveTypeId")]
        public virtual LeaveType? LeaveType { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Total Days")]
        public int NumberOfDays { get; set; }

        // Rules 7 & 10: Status tracking workflow matrix state values
        [Required]
        public string Status { get; set; } = "Pending"; // Default status state initialization

        [StringLength(500)]
        public string? Comments { get; set; }
    }
}
