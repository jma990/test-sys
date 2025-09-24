using Microsoft.EntityFrameworkCore;
using Content_Management_System.Data;  
using System.Diagnostics;

namespace Content_Management_System.Utilities
{
    public class SuperAdminService
    {
        private readonly AppDbContext _db;
        public SuperAdminService(AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateSuperAdmin()
        {
            // Check if an super admin already exists
            bool superAdminExists = await _db.Users.AnyAsync(u => u.Role == UserRole.SuperAdmin);
            if (superAdminExists) return;

            // Ensure "Admin" department exists
            var adminDept = await _db.Departments.FirstOrDefaultAsync(d => d.DepartmentName == "Admin");
            if (adminDept == null)
            {
                adminDept = new Department
                {
                    DepartmentName = "Super Admin",
                    IsActive = true
                };
                _db.Departments.Add(adminDept);
                await _db.SaveChangesAsync();  
            }

            // Create default super admin
            var salt = HashPassword.GenerateSalt();
            var superAdmin = new User
            {
                FirstName = "John Matthew",
                MiddleName = "Nemis",
                LastName = "Austria",
                Email = "jmaustria665@gmail.com",
                Password = HashPassword.This("123456789", salt),
                Salt = salt,
                Role = UserRole.SuperAdmin,
                CreatedAt = DateTime.Now,
                MustChangePassword = false,
                DepartmentID = adminDept.ID
            };
            _db.Users.Add(superAdmin);
            await _db.SaveChangesAsync();
        }
    }
}