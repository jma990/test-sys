using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography; 
using System.ComponentModel.DataAnnotations;
using Content_Management_System.Data;
using Content_Management_System.Utilities;
using Content_Management_System.PageFilters;

namespace Content_Management_System.Pages
{
    [ServiceFilter(typeof(AuthPageFilter))]
    public class AccountCreationModel : PageModel
    {
        private readonly AppDbContext _db;

        public AccountCreationModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        [Required(ErrorMessage = "First name is required.")]
        public required string FirstName { get; set; } = string.Empty;

        [BindProperty]
        public string? MiddleName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Last name is required.")]
        public required string LastName { get; set; } = string.Empty;

        [BindProperty]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Required(ErrorMessage = "Email is required.")]
        public required string Email { get; set; } = string.Empty;

        [BindProperty]
        public UserRole Role { get; set; } 

        [BindProperty]
        public int DepartmentID { get; set; }

        public List<Department> Departments { get; set; } = new();

        public async Task OnGetAsync()
        {
            Departments = await _db.Departments
                .Where(d => d.IsActive && d.ID != 1) // exclude ID 1 aka super admin
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Departments = await _db.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            if (!ModelState.IsValid) return Page();

            // Identify current logged-in user's role
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole == null)
            {
                TempData["Message"] = "Unauthorized. Please log in.";
                return RedirectToPage(PathDirectory.LoginPage);
            }

            // Restrict role creation
            if (currentUserRole == UserRole.Admin.ToString() && Role != UserRole.Member)
            {
                TempData["Message"] = "Admins can only create Members.";
                return Page();
            }

            // Normalize email for case-insensitive check
            var normalizedEmail = Email.ToLower();

            var existingUser = await _db.Users
                .AnyAsync(u => u.Email.ToLower() == normalizedEmail);

            if (existingUser)
            {
                TempData["Message"] = "Email is already in use.";
                return Page();
            }


            // Generate random secure password
            string tempPassword = RandomPasswordGenerator.Generate(10);
            string salt = HashPassword.GenerateSalt();
            string hashedPassword = HashPassword.This(tempPassword, salt);

            var user = new User
            {
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                Email = Email,
                Role = Role,
                DepartmentID = DepartmentID,
                Password = hashedPassword,
                Salt = salt,
                MustChangePassword = true
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["Message"] = $"User created successfully. Temporary password is: {tempPassword}";
            return RedirectToPage();
        }
    }
}
