using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Content_Management_System.Data;

namespace Content_Management_System.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _db;

        public DashboardModel(AppDbContext db)
        {
            _db = db;
        }

        // Holds the announcements
        public List<Announcement> Announcements { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await LoadAnnouncementsAsync();
        }

        public async Task<IActionResult> OnPostAddAnnouncementAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAnnouncementsAsync();
                return Page();
            }
            
            // Get logged-in user's ID from claims
            var userIDClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIDClaim == null)
            {
                // No user logged in — force logout (failsafe)
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToPage(PathDirectory.LoginPage);
            }

            int userID = int.Parse(userIDClaim.Value);

            var announcement = new Announcement
            {
                Title = Title,
                Content = Content,
                AuthorID = userID, 
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _db.Announcements.Add(announcement);
            await _db.SaveChangesAsync();

            return RedirectToPage(); // reload page and show new announcement
        }


        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage(PathDirectory.LoginPage);
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