using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Content_Management_System.Data;
using Content_Management_System.Utilities;
using Content_Management_System.PageFilters;

namespace Content_Management_System.Pages
{
    public class ChangePersonalInfoModel (AppDbContext db) : PageModel
    {
        private readonly AppDbContext _db = db;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "First name is required.")]
            [StringLength(50)]
            public string FirstName { get; set; } = string.Empty;

            [StringLength(50)]
            public string? MiddleName { get; set; }

            [Required(ErrorMessage = "Last name is required.")]
            [StringLength(50)]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Enter a valid email address.")]
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage(PathDirectory.LoginPage);
            }

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage(PathDirectory.LoginPage);
            }

            Input = new InputModel
            {
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage(PathDirectory.LoginPage);
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage(PathDirectory.LoginPage);
            }

            var normalizedEmail = Input.Email.ToLower();
            var existingUser = await _db.Users
                .AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.ID != userId);

            if (existingUser)
            {
                TempData["ErrorMessage"] = "Email is already in use.";
                return RedirectToPage();
            }

            // Update basic profile info
            user.FirstName = Input.FirstName;
            user.MiddleName = Input.MiddleName;
            user.LastName = Input.LastName;
            user.Email = Input.Email;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            // Refresh cookie with updated info
            await CookieService.CreateCookieAsync(HttpContext, user.ID, user.Email, user.Role.ToString(), user.MustChangePassword);

            TempData["SuccessMessage"] = "Account updated successfully.";
            return RedirectToPage();
        }
    }
}
