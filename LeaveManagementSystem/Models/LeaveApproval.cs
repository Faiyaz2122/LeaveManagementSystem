using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagementSystem.Models
{
    public class LeaveApproval
    {
        [Key]
        public int LeaveApprovalId { get; set; }

        [Required]
        public int LeaveRequestId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ActionDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = null!; // Approved or Rejected

        [StringLength(250)]
        [Display(Name = "Manager Remarks")]
        public string? Remarks { get; set; }

        // Navigation Properties
        [ForeignKey("LeaveRequestId")]
        public virtual LeaveRequest? LeaveRequest { get; set; }
    }
}
