using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Content_Management_System.Data;
using Content_Management_System.PageFilters;

namespace Content_Management_System.Pages
{
    [ServiceFilter(typeof(AuthPageFilter))]
    public class AnnouncementsModel : PageModel
    {
        private readonly AppDbContext _db;
        public AnnouncementsModel(AppDbContext db)
        {
            _db = db;
        }

        // For knowing which filter is active
        public string? ActiveFilter { get; set; }

        // Data containers
        public List<Announcement> RecentAnnouncements { get; set; } = new();
        public List<Announcement> ImportantAnnouncements { get; set; } = new();
        public List<Announcement> ArchivedAnnouncements { get; set; } = new();

        public async Task OnGetAsync(string? filter)
        {
            ActiveFilter = filter ?? "recent";

            if (ActiveFilter == "important")
            {
                ImportantAnnouncements = await _db.Announcements
                    .Include(a => a.Author)
                    .Include(a => a.Department)
                    .Where(a => a.DepartmentID == null && a.IsActive)
                    .OrderByDescending(a => a.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else if (ActiveFilter == "archived")
            {
                ArchivedAnnouncements = await _db.Announcements
                    .Include(a => a.Author)
                    .Include(a => a.Department)
                    .Where(a => !a.IsActive)
                    .OrderByDescending(a => a.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else // Default = recent
            {
                // Get user ID from cookie 
                int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

                // Fetch user's department ID from the database
                var userDepartmentId = await _db.Users
                    .Where(u => u.ID == userId)
                    .Select(u => u.DepartmentID)
                    .FirstOrDefaultAsync();

                // Then show announcements for their department OR global ones
                RecentAnnouncements = await _db.Announcements
                    .Include(a => a.Author)
                    .Include(a => a.Department)
                    .Where(a => a.IsActive &&
                        (a.DepartmentID == null || a.DepartmentID == userDepartmentId))
                    .OrderByDescending(a => a.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }

        }
    }
}
