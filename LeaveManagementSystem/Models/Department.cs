using MiniExcelLibs.Attributes;

namespace LeaveManagementSystem.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        // This tells MiniExcel to read "Department Name" if it has a space!
        [ExcelColumn(Name = "Department Name")]
        public string DepartmentName { get; set; } = null!;

        // Navigation
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}