using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ServicePortal.Domain.Entities;

namespace ServicePortal.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TypeLeave> TypeLeaves { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ApprovalFlow> ApprovalFlows { get; set; }
        public DbSet<ApprovalAction> ApprovalActions { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<UserConfig> UserConfigs { get; set; }
        public DbSet<MemoNotification> MemoNotifications { get; set; }
        public DbSet<MemoNotificationDepartment> MemoNotificationDepartments { get; set; }
        public DbSet<AttachFiles> AttachFiles { get; set; }
        public DbSet<AttachFileRelation> AttachFileRelations {  get; set; }
        public DbSet<UserManageAttendance> UserManageAttendances { get; set; }
        public DbSet<UserManageAttendanceUser> UserManageAttendanceUsers { get; set; }
        public DbSet<HrManagements> HrManagements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin", Code = "superadmin" },
                new Role { Id = 2, Name = "HR Manager", Code = "HR_Manager" },
                new Role { Id = 3, Name = "HR", Code = "HR" },
                new Role { Id = 4, Name = "User", Code = "user" },
                new Role { Id = 5, Name = "Duyệt đơn nghỉ phép", Code = "leave_request.approval" },
                new Role { Id = 6, Name = "Duyệt đơn nghỉ phép tới HR", Code = "leave_request.approval_to_hr" },
                new Role { Id = 7, Name = "HR duyệt đơn nghỉ phép", Code = "leave_request.hr_approval" }
            );

            modelBuilder.Entity<TypeLeave>().HasData(
                new TypeLeave { Id = 1, Name = "Annual", Note = "type_leave.annual", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) },
                new TypeLeave { Id = 2, Name = "Personal", Note = "type_leave.personal", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) },
                new TypeLeave { Id = 3, Name = "Sick", Note = "type_leave.sick", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) },
                new TypeLeave { Id = 4, Name = "Wedding", Note = "type_leave.wedding", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) },
                new TypeLeave { Id = 5, Name = "Accident", Note = "type_leave.accident", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) },
                new TypeLeave { Id = 6, Name = "Other", Note = "type_leave.other", ModifiedBy = "HR", ModifiedAt = new DateTime(2025, 5, 1, 15, 30, 0) }
            );

            //user - role
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserCode)
                .HasPrincipalKey(u => u.UserCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .HasPrincipalKey(r => r.Id)
                .OnDelete(DeleteBehavior.Cascade);

            //role - permision
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .HasPrincipalKey(r => r.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttachFileRelation>()
                .HasOne(x => x.AttachFiles)
                .WithMany(x => x.AttachFileRelations)
                .HasForeignKey(x => x.AttachFileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
