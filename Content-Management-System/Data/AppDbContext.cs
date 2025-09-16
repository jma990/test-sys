using Microsoft.EntityFrameworkCore;

namespace Content_Management_System.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Announcement> Announcements { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
    }

    public enum UserRole
    {
        Admin = 1,
        Member = 2
    }

    public class User
    {
        public int ID { get; set; } 
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
        public required string Salt { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Member; 
        public DateTime CreatedAt { get; set; } = DateTime.Now; 

         // Link to Department
        public required int DepartmentID { get; set; }
        public Department? Department { get; set; }

        // A user can post multiple announcements
        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
    }

    public class Announcement
    {
        public int ID { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public int AuthorID { get; set; }
        public User? Author { get; set; }
        public bool IsActive { get; set; } = true;
        public byte[]? Attachment { get; set; }

        // Optional link to Department (for department-specific announcements)
        public int? DepartmentID { get; set; }
        public Department? Department { get; set; }
    }

    public class Department
    {
        public int ID { get; set; }
        public required string DepartmentName { get; set; }

        // One department can have many users
        public ICollection<User> Users { get; set; } = new List<User>();

        // One department can have many announcements
        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
    }
}
