using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Content_Management_System.Data;


namespace Content_Management_System.Pages
{
    public class AnnouncementsModel : PageModel
    {
        private readonly AppDbContext _db;

        public AnnouncementsModel(AppDbContext db)
        {
            _db = db;
        }

        // Holds the announcements
        public List<Announcement> Announcements { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadAnnouncementsAsync();
            
        }
        // ----------------- Helpers -----------------

        private async Task LoadAnnouncementsAsync()
        {
            Announcements = await _db.Announcements
                .AsNoTracking()
                .Include(a => a.Author)
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

    }
}