using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace LeaveManagementSystem.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee name is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Please select a department.")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [Display(Name = "Assigned Privilege Level")]
        public int? RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role? SystemRole { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = "123456";

        // ==========================================
        // PLACED HERE: NULLABLE USERNAME MAPPING PROPERTY
        // ==========================================
        [Display(Name = "System Username (Optional)")]
        [StringLength(100)]
        public string? Username { get; set; }

        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
