using System.Data;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<UserConfig> UserConfigs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TypeLeave> TypeLeaves { get; set; }
        public DbSet<TimeLeave> TimeLeaves { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<HistoryApplicationForm> HistoryApplicationForms { get; set; }
        public DbSet<ApplicationForm> ApplicationForms { get; set; }
        public DbSet<MemoNotification> MemoNotifications { get; set; }
        public DbSet<MemoNotificationDepartment> MemoNotificationDepartments { get; set; }
        public DbSet<Domain.Entities.File> Files { get; set; }
        public DbSet<AttachFile> AttachFiles {  get; set; }
        public DbSet<RequestType> RequestTypes {  get; set; }
        public DbSet<RequestStatus> RequestStatuses {  get; set; }
        public DbSet<WorkFlowStep> WorkFlowSteps {  get; set; }

        public IDbConnection CreateConnection() => Database.GetDbConnection();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin", Code = "SUPERADMIN" },
                new Role { Id = 2, Name = "HR", Code = "HR" },
                new Role { Id = 3, Name = "IT", Code = "IT" },
                new Role { Id = 4, Name = "Union", Code = "UNION" },
                new Role { Id = 5, Name = "User", Code = "USER" }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "leave_request.create_leave_request", Description = "LEAVE_REQUEST" },
                new Permission { Id = 2, Name = "leave_request.send_to_hr", Description = "LEAVE_REQUEST" }
            );

            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { Id = 1, RoleId = 1, PermissionId = 1 },
                new RolePermission { Id = 2, RoleId = 2, PermissionId = 1 },
                new RolePermission { Id = 3, RoleId = 3, PermissionId = 1 },
                new RolePermission { Id = 4, RoleId = 4, PermissionId = 1 },
                new RolePermission { Id = 5, RoleId = 5, PermissionId = 1 }
            );

            modelBuilder.Entity<TypeLeave>().HasData(
                new TypeLeave { Id = 1, Name = "Annual", Note = "type_leave.annual" },
                new TypeLeave { Id = 2, Name = "Personal", Note = "type_leave.personal" },
                new TypeLeave { Id = 3, Name = "Sick", Note = "type_leave.sick" },
                new TypeLeave { Id = 4, Name = "Wedding", Note = "type_leave.wedding" },
                new TypeLeave { Id = 5, Name = "Accident", Note = "type_leave.accident" },
                new TypeLeave { Id = 6, Name = "Other", Note = "type_leave.other" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    UserCode = "0",
                    Password = "$2a$12$GAJGsDDQUCEPfSqOLbPwmu5agSkYoaH6eUzLPJLRx2hnA89LSkiey", //123456
                    IsChangePassword = 1,
                    IsActive = 1,
                    Email = "superadmin@vsvn.com.vn",
                    Phone = "0987654321",
                }
            );

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = 1,
                    UserCode = "0",
                    RoleId = 1
                }
            );

            //file - attach_file
            modelBuilder.Entity<AttachFile>()
                .HasOne(a => a.File)
                .WithMany()
                .HasForeignKey(a => a.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            //user → user_config
            modelBuilder.Entity<UserConfig>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserConfigs)
                .HasForeignKey(uc => uc.UserCode)
                .HasPrincipalKey(u => u.UserCode)
                .OnDelete(DeleteBehavior.Cascade);

            //User → UserRole
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserCode)
                .HasPrincipalKey(u => u.UserCode)
                .OnDelete(DeleteBehavior.Cascade);

            // Role → UserRole
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .HasPrincipalKey(r => r.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Role → RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .HasPrincipalKey(r => r.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Permission → RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // User → UserPermission
            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(up => up.UserCode)
                .HasPrincipalKey(u => u.UserCode)
                .OnDelete(DeleteBehavior.Cascade);

            // Permission → UserPermission
            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(up => up.PermissionId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

            //application_from - history_application_form
            modelBuilder.Entity<HistoryApplicationForm>()
                .HasOne(h => h.ApplicationForm)
                .WithMany()
                .HasForeignKey(h => h.ApplicationFormId)
                .OnDelete(DeleteBehavior.Cascade);

            //memo - memo dept
            modelBuilder.Entity<MemoNotificationDepartment>()
                .HasOne(mnd => mnd.MemoNotifications)
                .WithMany(mn => mn.MemoNotificationDepartments)
                .HasForeignKey(mnd => mnd.MemoNotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            //leave_request - type_leave
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.TypeLeave)
                .WithMany()
                .HasForeignKey(lr => lr.TypeLeaveId)
                .IsRequired(false);

            //leave_request - time_leave
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.TimeLeave)
                .WithMany()
                .HasForeignKey(lr => lr.TimeLeaveId)
                .IsRequired(false);

            //leave_request - application_form
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ApplicationForm)
                .WithMany()
                .HasForeignKey(lr => lr.ApplicationFormId)
                .IsRequired(false);

            //leave_request - user
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.User)
                .WithMany()
                .HasForeignKey(lr => lr.RequesterUserCode) 
                .HasPrincipalKey(u => u.UserCode) 
                .IsRequired(false);
        }
    }
}
