using Microsoft.EntityFrameworkCore;

namespace Content_Management_System.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Announcement> Announcements { get; set; } = null!;
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
    }
}
