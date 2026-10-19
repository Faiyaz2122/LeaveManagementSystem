using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;

namespace LeaveManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly LeaveDbContext _context;

        public DepartmentController(LeaveDbContext context)
        {
            _context = context;
        }

        // GET: Department
        public IActionResult Index()
        {
            var departments = _context.Departments.ToList();
            return View(departments);
        }

        // GET: Department/Details/5
        public IActionResult Details(int id)
        {
            var department = _context.Departments.Find(id);
            if (department == null) return NotFound();
            return View(department);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Department/Edit/5
        public IActionResult Edit(int id)
        {
            var department = _context.Departments.Find(id);
            if (department == null) return NotFound();
            return View(department);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Update(department);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Department/Delete/5
        public IActionResult Delete(int id)
        {
            var department = _context.Departments.Find(id);
            if (department == null) return NotFound();
            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var department = _context.Departments.Find(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // PIPELINE ACTION: DOWNLOAD EXCEL REPORT
        // ==========================================
        [HttpGet]
        public IActionResult ExportToExcel()
        {
            var departments = _context.Departments
                .Select(d => new { ID = d.DepartmentId, Name = d.DepartmentName })
                .ToList();

            var memoryStream = new MemoryStream();
            memoryStream.SaveAs(departments);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Departments_Report.xlsx");
        }

        // ==========================================
        // PIPELINE ACTION: GENERATE PDF EXPORT DOCUMENT
        // ==========================================
        // ==========================================
        // PIPELINE ACTION: GENERATE PDF EXPORT DOCUMENT
        // ==========================================
        [HttpGet]
        public IActionResult ExportToPdf()
        {
            // FIXED: Fully qualified namespace paths prevent CS0103 context errors
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var departments = _context.Departments.ToList();

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Leave Management System - Departments Report").FontSize(18).Bold().FontColor(Colors.Blue.Medium);

                    page.Content().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(4);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("ID").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Department Name").Bold();
                        });

                        foreach (var dept in departments)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(dept.DepartmentId.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(dept.DepartmentName);
                        }
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "Departments_Report.pdf");
        }

        // ==========================================
        // PIPELINE ACTION: UPLOAD AND PARSE SPREADSHEET (WARNING-FREE)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImportFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["FileError"] = "Please select a valid Excel file to import.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using (var stream = excelFile.OpenReadStream())
                {
                    // Read dynamically as a list of dictionary models
                    var rows = stream.Query(useHeaderRow: true).ToList();
                    int importedCount = 0;

                    foreach (var rowObj in rows)
                    {
                        // Explicitly cast to nullable dictionary layout to satisfy context compiler rule checking
                        var row = rowObj as IDictionary<string, object>;
                        if (row == null) continue;

                        string? departmentName = null;

                        if (row.ContainsKey("DepartmentName"))
                        {
                            departmentName = row["DepartmentName"]?.ToString();
                        }
                        else if (row.ContainsKey("Department Name"))
                        {
                            departmentName = row["Department Name"]?.ToString();
                        }
                        else
                        {
                            // Marked matchingKey as nullable type string? to resolve implicit mapping warnings
                            string? matchingKey = row.Keys.FirstOrDefault(k =>
                                k.Replace(" ", "").Equals("DepartmentName", StringComparison.OrdinalIgnoreCase));

                            if (matchingKey != null)
                            {
                                departmentName = row[matchingKey]?.ToString();
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(departmentName))
                        {
                            var cleanDept = new Department { DepartmentName = departmentName.Trim() };
                            _context.Departments.Add(cleanDept);
                            importedCount++;
                        }
                    }

                    if (importedCount > 0)
                    {
                        _context.SaveChanges();
                        TempData["FileSuccess"] = $"{importedCount} departments parsed and imported successfully!";
                    }
                    else
                    {
                        TempData["FileError"] = "No rows could be imported. Please verify your file column header is named 'Department Name' or 'DepartmentName'.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["FileError"] = $"Import failed unexpectedly: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
