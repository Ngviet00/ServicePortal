using Microsoft.EntityFrameworkCore;
using ServicePortal.Domain.Entities;

namespace ServicePortal.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        //public DbSet<LeaveRequest> LeaveRequests { get; set; }
        //public DbSet<ApprovalLeaveRequestStep> ApprovalLeaveRequestSteps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Name = "IT/MIS",
                    Note = "IT",
                    ParentId = null
                }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin" },
                new Role { Id = 2, Name = "IT" },
                new Role { Id = 3, Name = "HR" },
                new Role { Id = 4, Name = "User" }
            );

            modelBuilder.Entity<Position>().HasData(
                new Position
                {
                    Id = 1,
                    Name = "General Director",
                    Title = "General Director",
                    DepartmentId = 0,
                    Level = -2,
                    IsGlobal = true,
                },
                new Position
                {
                    Id = 2,
                    Name = "Assistant General Director",
                    Title = "Assistant General Director",
                    DepartmentId = 0,
                    Level = -1,
                    IsGlobal = true,
                },
                new Position
                {
                    Id = 3,
                    Name = "Superadmin",
                    Title = "Superadmin",
                    DepartmentId = 1,
                    Level = 0,
                    IsGlobal = true,
                },
                new Position
                {
                    Id = 4,
                    Name = "Manager",
                    Title = "Manager IT/MIS",
                    DepartmentId = 1,
                    Level = 1,
                    IsGlobal = false,
                }
            );
        }
    }
}
