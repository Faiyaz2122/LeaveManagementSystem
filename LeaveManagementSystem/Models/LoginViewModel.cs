using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please select an identity target profile.")]
        public int SelectedEmployeeId { get; set; }

        [Required(ErrorMessage = "Please select an access clearance role.")]
        public string AccessRole { get; set; } = "Employee"; // Options: "Employee" or "Manager"
    }
}
