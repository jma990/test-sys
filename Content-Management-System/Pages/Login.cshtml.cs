using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations; 
using Content_Management_System.Data;

namespace Content_Management_System.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public required string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; } 

        public string? ErrorMessage { get; set; } 

        public IActionResult OnPost()
        {
            // Hardcoded admin credentials for now
            const string adminEmail = "admin@gmail.com";
            const string adminPass = "123";

            if (Email == adminEmail && Password == adminPass)
            {
               
                // Redirect to homepage (or dashboard later)
                return RedirectToPage(PathDirectory.DashboardPage);
            }

            return Page();
        }
    }
}
