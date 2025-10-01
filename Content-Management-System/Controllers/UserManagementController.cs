using Microsoft.AspNetCore.Mvc;
using Content_Management_System.Data;
using Content_Management_System.Utilities;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Content_Management_System.Controllers
{
    [Route("UserManagement")]
    [ApiController]
    public class UserManagementController : Controller
    {
        private readonly AppDbContext _db;

        public UserManagementController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("VerifyPassword")]
        public async Task<IActionResult> OnPostVerifyPassword([FromBody] Data data)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Received data: {data.UserID} & {data.Password}");
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new { success = false, message = "Not logged in." });
            }

            int.TryParse(currentUserId, out int loggedInId);
            var loggedInUser = await _db.Users.FirstOrDefaultAsync(u => u.ID == loggedInId);
            if (loggedInUser == null)
            {
                return Unauthorized(new { success = false, message = "User not found." });
            }
            var enteredHash = HashPassword.This(data.Password, loggedInUser.Salt);

            if (enteredHash != loggedInUser.Password)
            {
                return Unauthorized(new { success = false, message = "Invalid password. Try again." });
            }

            Console.WriteLine($"Password verified by user {loggedInUser.Email}. Requested reset for UserID: {data.UserID}");

            var targetUser = await _db.Users.FirstOrDefaultAsync(u => u.ID == data.UserID);
            if (targetUser == null)
            {
                return NotFound(new { success = false, message = "Target user not found." });
            }
            var newPassword = RandomPasswordGenerator.Generate(12);
            var newSalt = HashPassword.GenerateSalt();
            var newHash = HashPassword.This(newPassword, newSalt);
            targetUser.Password = newHash;
            targetUser.Salt = newSalt;
            targetUser.MustChangePassword = true;

            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Password reset successful.", newPassword });
        }

        public class Data
        {
            public int UserID { get; set; } = 0;
            public required string Password { get; set; } = string.Empty;
        }
    }
}
