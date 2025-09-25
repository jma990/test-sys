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
    public class ChangePasswordModel (AppDbContext db) : PageModel
    {
        private readonly AppDbContext _db = db;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "Current password is required.")]
            public string? CurrentPassword { get; set; }

            [DataType(DataType.Password)]
            [Required(ErrorMessage = "New password is required.")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
                ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Required(ErrorMessage = "Please write new password.")]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string? ConfirmPassword { get; set; }
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

            var currentHashed = HashPassword.This(Input.CurrentPassword ?? string.Empty, user.Salt);
            if (currentHashed != user.Password)
            {
                ModelState.AddModelError("Input.CurrentPassword", "Current password is incorrect.");
                return Page();
            }

            var newHashedCheck = HashPassword.This(Input.NewPassword ?? string.Empty, user.Salt);
            if (newHashedCheck == user.Password)
            {
                ModelState.AddModelError("Input.NewPassword", "New password cannot be the same as the old password.");
                return Page();
            }
            
            var newSalt = HashPassword.GenerateSalt();
            var newHashed = HashPassword.This(Input.NewPassword!, newSalt);

            user.Password = newHashed;
            user.Salt = newSalt;
            user.MustChangePassword = false;  

            _db.Users.Update(user);
            await _db.SaveChangesAsync();
 
            await CookieService.CreateCookieAsync(HttpContext, user.ID, user.Email, user.Role.ToString(), user.MustChangePassword);

            TempData["SuccessMessage"] = "Password updated successfully.";
            return Page();
        }


    }
}
