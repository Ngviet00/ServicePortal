using Microsoft.EntityFrameworkCore;
using ServicePortal.Domain.Entities;

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
        public DbSet<PositionDeparment> PositionDeparments{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Deparment>().HasData(
                new Deparment
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
                new Position { Id = -1, Name = "SuperAdmin", PositionLevel = 0 },
                new Position { Id = 1, Name = "General Director", PositionLevel = 1 },
                new Position { Id = 2, Name = "Assistant General Director", PositionLevel = 2 },
                new Position { Id = 3, Name = "General Manager", PositionLevel = 3 },
                new Position { Id = 4, Name = "Manager", PositionLevel = 4 },
                new Position { Id = 5, Name = "Assistant Manager", PositionLevel = 5 },
                new Position { Id = 6, Name = "Supervisor", PositionLevel = 6 },
                new Position { Id = 7, Name = "Chief Accountant", PositionLevel = 6 },
                new Position { Id = 8, Name = "Staff", PositionLevel = 7 }
            );

            //init position in IT deparment, include Manager
            modelBuilder.Entity<PositionDeparment>().HasData(
                new PositionDeparment
                {
                    Id = 1,
                    DeparmentId = 1, //IT-MIS
                    PositionId = 4,  //Manager
                    PositionDeparmentLevel = 1,
                    CustomTitle = "Manager IT/MIS"
                }
            );
        }
    }
}
