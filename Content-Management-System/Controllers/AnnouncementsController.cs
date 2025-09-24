using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Diagnostics;
using Content_Management_System.Models;
using Content_Management_System.Data;
using Content_Management_System.Utilities;

namespace Content_Management_System.Controllers
{
    [Route("Announcements")]
    [ApiController]
    public class AnnouncementsController : Controller
    {
        private readonly AppDbContext _db;

        public AnnouncementsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("GetUsers")]
        public IActionResult GetUsers(int departmentId)
        {
            var users = _db.Users
                .Where(u => u.DepartmentID == departmentId)
                .Select(u => new 
                {
                    u.FirstName,
                    u.LastName
                })
                .ToList();

            return Ok(users);
        }


        [HttpGet("GetDepartments")]
        public IActionResult GetDepartments()
        {
            var departments = _db.Departments
                .Where(d => d.ID != 1) // exclude DepartmentID 1 (Super Admin)
                .Select(d => new { d.ID, d.DepartmentName })
                .ToList();

            return Ok(departments);
        }

        [HttpPost("AddAnnouncement")]
        public IActionResult OnPostAddAnnouncement([FromBody] Data data)
        {
            Console.WriteLine("===== New Announcement Submitted =====");
            Console.WriteLine($"Title: {data.Title}");
            Console.WriteLine($"Content: {data.Content}");
            Console.WriteLine($"DepartmentID: {data.DepartmentID}");
            Console.WriteLine("=====================================");

            if (string.IsNullOrWhiteSpace(data.Title))
            {
                return Ok(new { success = false, message = "Title is required." });
            }
            if (string.IsNullOrWhiteSpace(data.Content))
            {
                return Ok(new { success = false, message = "Content is required." });
            }
            if (data.DepartmentID < 0)
            {
                return Ok(new { success = false, message = "Please select a department." });
            }

            if (!string.IsNullOrWhiteSpace(data.Link))
            {
                if (!Uri.TryCreate(data.Link, UriKind.Absolute, out var uriResult) 
                    || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    return Ok(new { success = false, message = "Invalid link format. Please check your input." });
                }
            }
            try
            {
                var userIDClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIDClaim == null)
                {
                    return Unauthorized(new { success = false, message = "User not logged in." });
                }

                int authorID = int.Parse(userIDClaim.Value);
                var newAnnouncement = new Announcement
                {
                    Title = data.Title,
                    Content = data.Content,
                    DepartmentID = data.DepartmentID == 0 ? null : data.DepartmentID,
                    Link = string.IsNullOrWhiteSpace(data.Link) ? null : data.Link,
                    CreatedAt = TimezoneHelper.NowPH(),
                    IsActive = true,
                    AuthorID = authorID 
                };

                _db.Announcements.Add(newAnnouncement);
                _db.SaveChanges();

                return Ok(new { success = true, message = "Announcement successfully added!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        public class Data
        {
            public required string Title { get; set; } = string.Empty;
            public required string Content { get; set; } = string.Empty;
            public int DepartmentID { get; set; } = 0;
            public string? Link { get; set; } = string.Empty;
        }
    }
}
