using Microsoft.EntityFrameworkCore;
using Content_Management_System.Data;  
using System.Diagnostics;

namespace Content_Management_System.Utilities
{
    public class AdminService
    {
        private readonly AppDbContext _db;
        public AdminService(AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateAdmin()
        {
            // Check if an admin already exists
            bool adminExists = await _db.Users.AnyAsync(u => u.Role == UserRole.Admin);
            if (adminExists) return;

            // Ensure "Admin" department exists
            var adminDept = await _db.Departments.FirstOrDefaultAsync(d => d.DepartmentName == "Admin");
            if (adminDept == null)
            {
                adminDept = new Department
                {
                    DepartmentName = "Admin"
                };
                _db.Departments.Add(adminDept);
                await _db.SaveChangesAsync();  
            }

            // Create default admin
            var salt = HashPassword.GenerateSalt();
            var admin = new User
            {
                FirstName = "John Matthew",
                MiddleName = "Nemis",
                LastName = "Austria",
                Email = "jmaustria665@gmail.com",
                Password = HashPassword.This("123456789", salt),
                Salt = salt,
                Role = UserRole.Admin,
                CreatedAt = DateTime.Now,
                DepartmentID = adminDept.ID
            };
            _db.Users.Add(admin);
            await _db.SaveChangesAsync();
        }
    }
}