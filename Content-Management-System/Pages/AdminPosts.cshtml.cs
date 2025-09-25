using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Content_Management_System.Data;
using Content_Management_System.Utilities;
using Content_Management_System.PageFilters;

namespace Content_Management_System.Pages
{
    [ServiceFilter(typeof(AuthPageFilter))]
    public class AdminPostsModel : PageModel
    {
        private readonly AppDbContext _db;
        public AdminPostsModel(AppDbContext db)
        {
            _db = db;
        }

        public IList<Announcement> MyPosts { get; set; } = new List<Announcement>();
        public List<Department> Departments { get; set; } = new List<Department>();

        [BindProperty]
        public int EditID { get; set; }

        [BindProperty]
        public required string EditTitle { get; set; } = string.Empty;

        [BindProperty]
        public required string EditContent { get; set; } = string.Empty;

        [BindProperty]
        public string? EditLink { get; set; }

        [BindProperty]
        public bool EditIsActive { get; set; }

        // TempData for SweetAlert message
        public string? AlertMessage
        {
            get => TempData["AlertMessage"] as string;
            set => TempData["AlertMessage"] = value;
        }

        public string? AlertType
        {
            get => TempData["AlertType"] as string;
            set => TempData["AlertType"] = value;
        }

        // For filtering
        [BindProperty(SupportsGet = true)]
        public string? SelectedDepartmentID { get; set; } = "showAll";

        [BindProperty(SupportsGet = true)]
        public string SelectedStatus { get; set; } = "all"; 


        public async Task<IActionResult> OnGetAsync()
        {
            var userIDClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIDClaim == null) return RedirectToPage(PathDirectory.LoginPage);

            int authorID = int.Parse(userIDClaim.Value);

            // Get all posts by this author
            var query = _db.Announcements
                .Include(a => a.Department)
                .Where(a => a.AuthorID == authorID);

            if (!string.IsNullOrEmpty(SelectedDepartmentID))
            {
                if (SelectedDepartmentID == "showAll")
                {
                    // don’t filter anything
                }
                else if (SelectedDepartmentID == "showForAllDepartments")
                {
                    // posts that belong to any department (null DepartmentID)
                    query = query.Where(a => a.DepartmentID == null);
                }
                else if (int.TryParse(SelectedDepartmentID, out int deptId))
                {
                    query = query.Where(a => a.DepartmentID == deptId);
                }
            }

            if (!string.IsNullOrEmpty(SelectedStatus) && SelectedStatus != "all")
            {
                if (SelectedStatus == "active")
                    query = query.Where(a => a.IsActive);
                else if (SelectedStatus == "inactive")
                    query = query.Where(a => !a.IsActive);
            }

            MyPosts = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            // Departments for dropdown (only those with posts)
            Departments = await _db.Announcements
                .Include(a => a.Department)
                .Where(a => a.AuthorID == authorID && a.DepartmentID != null)
                .Select(a => a.Department!)
                .Distinct()
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            return Page();
        }


        public async Task<IActionResult> OnPostEditAsync()
        {
            var userIDClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIDClaim == null) return RedirectToPage("/Login");

            int authorID = int.Parse(userIDClaim.Value);

            // --- BACKEND VALIDATION ---
            if (string.IsNullOrWhiteSpace(EditTitle) || string.IsNullOrWhiteSpace(EditContent))
                return await ValidationError("Title and Content cannot be empty!", "error");

            if (!string.IsNullOrWhiteSpace(EditLink) && !IsValidUrl(EditLink))
                return await ValidationError("The provided link is invalid.", "error");
            // --------------------------

            var announcement = await _db.Announcements
                .FirstOrDefaultAsync(a => a.ID == EditID && a.AuthorID == authorID);
            if (announcement == null)
                return await ValidationError("Announcement not found. Contact Administrator.", "error");

            // --- SKIP IF NOTHING CHANGED ---
            var newLink = string.IsNullOrWhiteSpace(EditLink) ? null : EditLink;
            if (announcement.Title == EditTitle &&
                announcement.Content == EditContent &&
                announcement.Link == newLink &&
                announcement.IsActive == EditIsActive)
            {
                return await ValidationError("No changes detected.", "info");
            }
            // -------------------------------

            announcement.Title = EditTitle;
            announcement.Content = EditContent;
            announcement.Link = string.IsNullOrWhiteSpace(EditLink) ? null : EditLink;
            announcement.IsActive = EditIsActive;
            announcement.UpdatedAt = TimezoneHelper.NowPH();

            await _db.SaveChangesAsync();

            // Show success message
            AlertMessage = "Update successful!";
            AlertType = "success";

            return RedirectToPage(new { SelectedDepartmentID, SelectedStatus });
        }

        private async Task<IActionResult> ValidationError(string message, string type)
        {
            AlertMessage = message;
            AlertType = type;
            return await OnGetAsync();
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
