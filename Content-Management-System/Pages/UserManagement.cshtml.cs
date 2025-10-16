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

        [HttpPost]
        public async Task<IActionResult> OnPostToggleArchive([FromBody] ToggleArchiveRequest request)
        {
            if (request == null || request.UserID <= 0)
            {
                Console.WriteLine("Invalid or missing userID");
                return BadRequest(new { message = "Invalid userID" });
            }

            var user = await _db.Users.FindAsync(request.UserID);
            if (user == null)
            {
                Console.WriteLine($"User with ID {request.UserID} not found");
                return NotFound(new { message = "User not found" });
            }

            user.IsArchived = !user.IsArchived;  
            await _db.SaveChangesAsync();

            Console.WriteLine($"User with ID {request.UserID} has been {(user.IsArchived ? "archived" : "unarchived")}");
            return new JsonResult(new
            {
                message = $"User has been successfully {(user.IsArchived ? "archived" : "unarchived")}",
                isArchived = user.IsArchived
            });
        }

        public class ToggleArchiveRequest
        {
            public int UserID { get; set; }
        }
        
        [HttpPost]
        public async Task<IActionResult> OnPostChangeDepartmentAsync([FromBody] ChangeDepartmentRequest request)
        {
            if (request == null || request.UserId <= 0 || request.DepartmentId <= 0)
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            var user = await _db.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var department = await _db.Departments.FindAsync(request.DepartmentId);
            if (department == null || !department.IsActive)
            {
                return NotFound(new { message = "Department not found or inactive" });
            }

            user.DepartmentID = request.DepartmentId;
            await _db.SaveChangesAsync();

            return new JsonResult(new { message = $"Department changed successfully for {user.FirstName} {user.LastName}" });
        }

        public class ChangeDepartmentRequest
        {
            public int UserId { get; set; }
            public int DepartmentId { get; set; }
        }
    }
}
