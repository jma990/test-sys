using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Content_Management_System.Data;
using Content_Management_System.Utilities;


namespace Content_Management_System.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;

        public LoginModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public required string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; } 

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null)
            {
                ModelState.AddModelError("Password", "Invalid email or password.");
                return Page();
            }

            var hashedInput = HashPassword.This(Password, user.Salt);
            if (hashedInput != user.Password)
            {
                ModelState.AddModelError("Password", "Invalid email or password.");
                return Page();
            }
            await CookieService.CreateCookieAsync(HttpContext, user.ID, user.Email, user.Role.ToString());
            return RedirectToPage(PathDirectory.DashboardPage);
        }
    }
}
