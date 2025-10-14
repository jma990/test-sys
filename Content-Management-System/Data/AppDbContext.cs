using Microsoft.EntityFrameworkCore;

namespace Content_Management_System.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Announcement> Announcements { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<ResetPasswordLog> ResetPasswordLogs { get; set; } = null!;
    }

    public enum UserRole
    {
        SuperAdmin = 0,
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
        public bool MustChangePassword { get; set; } = true;
        public bool IsArchived { get; set; } = false;

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
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int AuthorID { get; set; }
        public User? Author { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Link { get; set; }
        public byte[]? Attachment { get; set; }
        
        // Optional link to Department (for department-specific announcements)
        public int? DepartmentID { get; set; }
        public Department? Department { get; set; }
    }

    public class Department
    {
        public int ID { get; set; }
        public required string DepartmentName { get; set; }
        public bool IsActive { get; set; }

        // One department can have many users
        public ICollection<User> Users { get; set; } = new List<User>();

        // One department can have many announcements
        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
    }

    public class ResetPasswordLog
    {
        public int ID { get; set; }

        // Admin who reset the password
        public int AdminID { get; set; }
        public User? Admin { get; set; }

        // Target user whose password was reset
        public int TargetUserID { get; set; }
        public User? TargetUser { get; set; }

        // Timestamp of reset
        public DateTime ResetAt { get; set; } = DateTime.Now;
    }
}
