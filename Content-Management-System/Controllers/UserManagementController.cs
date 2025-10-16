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

            var log = new ResetPasswordLog
            {
                AdminID = loggedInUser.ID,
                TargetUserID = targetUser.ID,
                ResetAt = DateTime.Now
            };
            _db.ResetPasswordLogs.Add(log);

            await _db.SaveChangesAsync();
            return Ok(new { success = true, message = "Password reset successful.", newPassword });
        }

        [HttpPost("ToggleArchive")]
        public IActionResult ToggleArchive([FromBody] ArchiveData data)
        {
            Console.WriteLine($"The userId received is: {data.UserID}");
            return Ok(new { success = true, message = $"UserID {data.UserID} received." });
        }

        public class Data
        {
            public int UserID { get; set; }
            public required string Password { get; set; }
        }

        public class ArchiveData
        {
            public int UserID { get; set; }
        }

        [HttpGet("GetDepartments")]
        public async Task<IActionResult> OnGetGetDepartmentsAsync()
        {
            var departments = await _db.Departments
                .Where(d => d.IsActive && d.ID != 1) // Exclude Super Admin (ID 1)
                .Select(d => new { id = d.ID, name = d.DepartmentName })
                .ToListAsync();

            return new JsonResult(departments);
        }

    }
}
