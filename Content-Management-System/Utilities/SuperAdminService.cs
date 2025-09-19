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

            // Seed default departments if they don't exist (TEMPORARY!!)
            await SeedDepartments();

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

        // DELETE THIS LATER (TEMPORARY!!)
        private async Task SeedDepartments()
        {
            var departments = new List<string>
            {
                "Admin",
                "Information Technology",
                "Computer Engineering",
                "General Department",
                "Test Department A",
                "Test Department B"
            };

            foreach (var deptName in departments)
            {
                if (!await _db.Departments.AnyAsync(d => d.DepartmentName == deptName))
                {
                    _db.Departments.Add(new Department { DepartmentName = deptName, IsActive = true });
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}