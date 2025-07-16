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
        public DbSet<AttachFile> AttachFiles { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<RequestStatus> RequestStatuses { get; set; }
        public DbSet<WorkFlowStep> WorkFlowSteps { get; set; }
        public DbSet<UserMngOrgUnitId> UserMngOrgUnits { get; set; }

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
                new Permission { Id = 2, Name = "leave_request.send_to_hr", Description = "LEAVE_REQUEST" },
                new Permission { Id = 3, Name = "time_keeping.mng_time_keeping", Description = "TIME_KEEPING" },
                new Permission { Id = 4, Name = "leave_request.create_multiple_leave_request", Description = "LEAVE_REQUEST" }
            );

            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { Id = 1, RoleId = 1, PermissionId = 1 },
                new RolePermission { Id = 2, RoleId = 2, PermissionId = 1 },
                new RolePermission { Id = 3, RoleId = 3, PermissionId = 1 },
                new RolePermission { Id = 4, RoleId = 4, PermissionId = 1 },
                new RolePermission { Id = 5, RoleId = 5, PermissionId = 1 }
            );

            modelBuilder.Entity<TypeLeave>().HasData(
                new TypeLeave { Id = 1, Name = "Annual", Note = "type_leave.annual", NameV = "Nghỉ Phép Năm"  },
                new TypeLeave { Id = 2, Name = "Personal", Note = "type_leave.personal", NameV = "Nghỉ Việc Cá Nhân" },
                new TypeLeave { Id = 3, Name = "Sick", Note = "type_leave.sick", NameV = "Nghỉ Ốm" },
                new TypeLeave { Id = 4, Name = "Wedding", Note = "type_leave.wedding", NameV = "Nghỉ Cưới" },
                new TypeLeave { Id = 5, Name = "Accident", Note = "type_leave.accident", NameV = "Nghỉ TNLĐ" },
                new TypeLeave { Id = 6, Name = "Other", Note = "type_leave.other", NameV = "Khác" }
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

            modelBuilder.Entity<TimeLeave>().HasData(
                new TimeLeave
                {
                    Id = 1,
                    Description = "Cả Ngày",
                    English = "All Day"
                },
                new TimeLeave
                {
                    Id = 2,
                    Description = "Buổi Sáng",
                    English = "Morning"
                },
                new TimeLeave
                {
                    Id = 3,
                    Description = "Buổi Chiều",
                    English = "Afternoon"
                }
            );

            modelBuilder.Entity<RequestStatus>().HasData(
                new RequestStatus
                {
                    Id = 1,
                    Name = "PENDING"
                },
                new RequestStatus
                {
                    Id = 2,
                    Name = "IN_PROCESS"
                },
                new RequestStatus
                {
                    Id = 3,
                    Name = "COMPLETE"
                },
                new RequestStatus
                {
                    Id = 4,
                    Name = "WAIT_HR"
                },
                new RequestStatus
                {
                    Id = 5,
                    Name = "REJECT"
                }
            );

            modelBuilder.Entity<RequestType>().HasData(
                new RequestType
                {
                    Id = 1,
                    Name = "Nghỉ Phép"
                },
                new RequestType
                {
                    Id = 2,
                    Name = "Chấm Công"
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
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //User → UserRole
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserCode)
                .HasPrincipalKey(u => u.UserCode)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Role → UserRole
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .HasPrincipalKey(r => r.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Role → RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasPrincipalKey(r => r.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Permission → RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasPrincipalKey(p => p.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // User → UserPermission
            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(up => up.UserCode)
                .HasPrincipalKey(u => u.UserCode)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Permission → UserPermission
            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.Permission)
                .WithMany(p => p.UserPermissions)
                .HasPrincipalKey(p => p.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //application_from - history_application_form
            modelBuilder.Entity<HistoryApplicationForm>()
                .HasOne(h => h.ApplicationForm)
                .WithMany(a => a.HistoryApplicationForms)
                .HasPrincipalKey(a => a.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //memo - memo dept
            modelBuilder.Entity<MemoNotificationDepartment>()
                .HasOne(mnd => mnd.MemoNotifications)
                .WithMany(mn => mn.MemoNotificationDepartments)
                .HasPrincipalKey(mn => mn.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //leave_request - type_leave
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.TypeLeave)
                .WithMany()
                .HasPrincipalKey(t => t.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //leave_request - time_leavecls
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.TimeLeave)
                .WithMany()
                .HasPrincipalKey(t => t.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //leave_request - application_form
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ApplicationForm)
                .WithMany()
                .HasPrincipalKey(a => a.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //leave_request - user
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.User)
                .WithMany()
                .HasPrincipalKey(u => u.UserCode)
                .HasForeignKey(lr => lr.RequesterUserCode)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
