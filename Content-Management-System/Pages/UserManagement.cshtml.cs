using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Content_Management_System.Data;
using Content_Management_System.PageFilters;

namespace Content_Management_System.Pages
{
    [ServiceFilter(typeof(AuthPageFilter))]
    public class UserManagementModel : PageModel
    {
        private readonly AppDbContext _db;

        public UserManagementModel(AppDbContext db)
        {
            _db = db;
        }

        public List<User> Users { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        { 
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(roleClaim) || !Enum.TryParse<UserRole>(roleClaim, out var currentUserRole))
            {
                return RedirectToPage(PathDirectory.LoginPage);
            }
 
            Users = await _db.Users
                .Include(u => u.Department)
                .Where(u => u.Role > currentUserRole)
                .OrderBy(u => u.Role)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetGetResetLogsAsync(int userId)
        {
            var logs = await _db.ResetPasswordLogs
                .Where(l => l.TargetUserID == userId)
                .Include(l => l.Admin)
                .OrderByDescending(l => l.ResetAt)
                .Select(l => new {
                    adminName = l.Admin != null ? $"{l.Admin.FirstName} {l.Admin.LastName}" : "Unknown",
                    resetAt = l.ResetAt
                })
                .ToListAsync();

            return new JsonResult(logs);
        }

    }
}
