using Microsoft.EntityFrameworkCore;
using ServicePortal.Domain.Entities;

namespace ServicePortal.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        //public DbSet<LeaveRequest> LeaveRequests { get; set; }
        //public DbSet<LeaveRequestStep> LeaveRequestSteps{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Name = "IT/MIS",
                    Note = "department.IT",
                    ParentId = null
                },
                new Department
                {
                    Id = 2,
                    Name = "HR",
                    Note = "department.HR",
                    ParentId = null
                },
                new Department
                {
                    Id = 3,
                    Name = "Sản xuất",
                    Note = "department.production",
                    ParentId = null
                }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin" },
                new Role { Id = 3, Name = "HR" },
                new Role { Id = 4, Name = "User" }
            );
        }
    }
}
