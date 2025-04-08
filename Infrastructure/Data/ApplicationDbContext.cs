using Microsoft.EntityFrameworkCore;
using ServicePortal.Domain.Models;
using ServicePortal.Domains.Models;

namespace ServicePortal.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<Deparment> Deparments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserAssignment> UserAssignments { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<ApprovalLeaveRequestStep> ApprovalLeaveRequestSteps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Deparment>().HasData(
                new Deparment
                {
                    Id = 1,
                    Name = "Business Development",
                    Note = "Phát triền kinh doanh",
                    ParentId = null
                },
                new Deparment
                {
                    Id = 2,
                    Name = "Commercial",
                    Note = "Thương mại",
                    ParentId = null
                }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin"},
                new Role { Id = 2, Name = "HR"},
                new Role { Id = 3, Name = "IT"},
                new Role { Id = 4, Name = "Office"},
                new Role { Id = 5, Name = "Production"}
            );

            modelBuilder.Entity<Position>().HasData(
                new Position { Id = 1, Name = "General Director", Level = 1 },
                new Position { Id = 2, Name = "Assistant General Director", Level = 2 },
                new Position { Id = 3, Name = "General Manager", Level = 3 },
                new Position { Id = 4, Name = "Manager", Level = 4 },
                new Position { Id = 5, Name = "Assistant Manager", Level = 5 },
                new Position { Id = 6, Name = "Supervisor", Level = 6 },
                new Position { Id = 7, Name = "Chief Accountant", Level = 6 },
                new Position { Id = 8, Name = "Staff", Level = 7 },
                new Position { Id = 9, Name = "Operator", Level = 15 }
            );
        }
    }
}
