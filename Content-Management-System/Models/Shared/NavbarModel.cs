using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Content_Management_System.Data;

namespace Content_Management_System.Models.Shared
{
    public class NavbarModel
    {
        private readonly AppDbContext _db;

        public string? UserName { get; set; }
        public string? Role { get; set; }

        public NavbarModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task LoadUserDataAsync(ClaimsPrincipal user)
        {
            if (user.Identity?.IsAuthenticated == true)
            {
                var email = user.Identity.Name;

                var account = await _db.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (account != null)
                {
                    UserName = account.FirstName + " " + account.LastName;
                    Role = account.Role.ToString();
                }
            }
        }
    }
}
