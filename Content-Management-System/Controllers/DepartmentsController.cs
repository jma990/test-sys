using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Diagnostics;
using Content_Management_System.Models;
using Content_Management_System.Data;
using Content_Management_System.Utilities;

namespace Content_Management_System.Controllers
{
    [Route("Departments")]
    [ApiController]
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _db;

        public DepartmentsController(AppDbContext db)
        {
            _db = db;
        }
 
        [HttpPost("AddDepartment")]
        public IActionResult OnPostAddDEpartment([FromBody] Data data)
        {
            Console.WriteLine("===== New Department Submitted =====");
            Console.WriteLine($"Name of Department: {data.DepartmentName}");
            Console.WriteLine($"IsActive: {data.IsActive}");

            if (string.IsNullOrWhiteSpace(data.DepartmentName))
            {
                return Ok(new { success = false, message = "Name is required." });
            }

            var exists = _db.Departments
                .Any(d => d.DepartmentName.ToLower() == data.DepartmentName.Trim().ToLower());

            if (exists)
            {
                return Ok(new { success = false, message = "Department already exists." });
            }

            var department = new Department
            {
                DepartmentName = data.DepartmentName.Trim(),
                IsActive = data.IsActive
            };

            _db.Departments.Add(department);
            _db.SaveChanges();
              
            return Ok(new { success = true, message = "Department successfully added!" }); 
        }

        public class Data
        {
            public required string DepartmentName { get; set; } = string.Empty; 
            public bool IsActive { get; set; } = false; 
        }
    }
}
