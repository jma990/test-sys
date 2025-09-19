using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Content_Management_System.Data;
using Content_Management_System.Utilities;

namespace Content_Management_System.Pages
{
    public class MandatoryPasswordChangeModel : PageModel
    {
        private readonly AppDbContext _db;

        public MandatoryPasswordChangeModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage(PathDirectory.LoginPage);
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null)
            {
                return RedirectToPage(PathDirectory.LoginPage);
            }

            if (!user.MustChangePassword)
            {
                return RedirectToPage(PathDirectory.AnnouncementsPage);
            }

            return Page(); 
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Get current logged-in user
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage(PathDirectory.LoginPage);
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null)
                return RedirectToPage(PathDirectory.LoginPage);

            // Generate new salt + hash
            var newSalt = HashPassword.GenerateSalt();
            var newHashedPassword = HashPassword.This(NewPassword, newSalt);

            user.Password = newHashedPassword;
            user.Salt = newSalt;
            user.MustChangePassword = false;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            // Refresh cookie so it has updated MustChangePassword = false
            await CookieService.CreateCookieAsync(HttpContext, user.ID, user.Email, user.Role.ToString(), user.MustChangePassword);

            // Redirect to announcements
            return RedirectToPage(PathDirectory.AnnouncementsPage);
        }
    }
}
