using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LeaveManagementSystem.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Clearance Designation")]
        public string RoleName { get; set; } = null!; // E.g., "Manager", "Employee", "HR-Admin"

        // Navigation reference collection
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
