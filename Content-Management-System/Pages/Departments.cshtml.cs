using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Content_Management_System.Data;
using Content_Management_System.PageFilters;

namespace Content_Management_System.Pages
{
    [ServiceFilter(typeof(AuthPageFilter))]
    public class DepartmentsModel : PageModel
    {
        private readonly AppDbContext _db;

        public DepartmentsModel(AppDbContext db)
        {
            _db = db;
        }

        public List<DepartmentViewModel> Departments { get; set; } = new();

        [BindProperty]
        public int EditID { get; set; }

        [BindProperty]
        public string EditDepartmentName { get; set; } = string.Empty;

        [BindProperty] 
        public int ArchiveID { get; set; }

        // TempData for SweetAlert message
        [TempData]
        public string? AlertMessage { get; set; }

        [TempData]
        public string? AlertType { get; set; } 

        public async Task OnGetAsync()
        {
            Departments = await _db.Departments
                .Where(d => d.DepartmentName.ToLower() != "super admin")
                .OrderBy(d => d.DepartmentName)
                .Select(d => new DepartmentViewModel
                {
                    ID = d.ID,
                    DepartmentName = d.DepartmentName,
                    UserCount = d.Users.Count,
                    IsActive = d.IsActive
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (string.IsNullOrWhiteSpace(EditDepartmentName))
            {
                AlertMessage = "Department name cannot be empty.";
                AlertType = "error";
                return RedirectToPage();
            }

            var dept = await _db.Departments.FirstOrDefaultAsync(d => d.ID == EditID);
            if (dept == null)
            {
                AlertMessage = "Department not found.";
                AlertType = "error";
                return RedirectToPage();
            }

            if (dept.DepartmentName == EditDepartmentName)
            {
                AlertMessage = "No changes detected.";
                AlertType = "info";
                return RedirectToPage();
            }

            dept.DepartmentName = EditDepartmentName;
            await _db.SaveChangesAsync();

            AlertMessage = "Department updated successfully!";
            AlertType = "success";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostArchiveAsync()
        {
            var dept = await _db.Departments
                .Include(d => d.Users)  
                .FirstOrDefaultAsync(d => d.ID == ArchiveID);

            if (dept == null)
            {
                AlertMessage = "Department not found.";
                AlertType = "error";
                return RedirectToPage();
            }

            if (dept.IsActive)
            {
                // Archive
                if (dept.Users.Any())
                {
                    AlertMessage = "Department cannot be archived because it has assigned users.";
                    AlertType = "error";
                    return RedirectToPage();
                }

                dept.IsActive = false;
                AlertMessage = "Department archived successfully!";
                AlertType = "success";
            }
            else
            {
                // Unarchive
                dept.IsActive = true;
                AlertMessage = "Department unarchived successfully!";
                AlertType = "success";
            }

            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
    }

    public class DepartmentViewModel
    {
        public int ID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public bool IsActive { get; set; }
    }
}
