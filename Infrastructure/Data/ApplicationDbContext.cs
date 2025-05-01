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
        public DbSet<TypeLeave> TypeLeaves { get; set; }

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

            modelBuilder.Entity<TypeLeave>().HasData(
                new TypeLeave { Id = 1, Name = "Annual", Note = "type_leave.annual", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) },
                new TypeLeave { Id = 2, Name = "Personal", Note = "type_leave.personal", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) },
                new TypeLeave { Id = 3, Name = "Sick", Note = "type_leave.sick", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) },
                new TypeLeave { Id = 4, Name = "Wedding", Note = "type_leave.wedding", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) },
                new TypeLeave { Id = 5, Name = "Accident", Note = "type_leave.accident", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) },
                new TypeLeave { Id = 6, Name = "Other", Note = "type_leave.other", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) }
            );
        }
    }
}
