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
                    .Where(a => a.DepartmentID == null && a.IsActive)
                    .OrderByDescending(a => a.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else if (ActiveFilter == "archived")
            {
                ArchivedAnnouncements = await _db.Announcements
                    .Include(a => a.Author)
                    .Where(a => !a.IsActive)
                    .OrderByDescending(a => a.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else // Default = recent
            {
                RecentAnnouncements = await _db.Announcements
                    .Include(a => a.Author)
                    .Where(a => a.IsActive)
                    .OrderByDescending(a => a.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
    }
}
