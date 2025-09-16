using Microsoft.AspNetCore.Mvc;
using Content_Management_System.Models;
using Content_Management_System.Data;
using System.Diagnostics;

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

        [HttpPost("AddAnnouncement")]
        [ValidateAntiForgeryToken]
        public IActionResult OnPostAddAnnouncement([FromBody] Data data)
        {
            Console.WriteLine("===== New Announcement Submitted =====");
            Console.WriteLine($"Title: {data.Title}");
            Console.WriteLine($"Content: {data.Content}");
            Console.WriteLine($"DepartmentID: {data.DepartmentID}");
            Console.WriteLine("=====================================");

            return Json(new { success = true, message = "Announcement received!" });
        }

        public class Data
        {
            public string Title { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
            public int DepartmentID { get; set; }
        }
    }
}
