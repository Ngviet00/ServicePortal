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
        public DbSet<ApprovalFlow> ApprovalFlows { get; set; }
        public DbSet<UserMngOrgUnitId> UserMngOrgUnitId { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<OrgUnit> OrgUnits { get; set; }
        public DbSet<OrgPosition> OrgPositions { get; set; }
        public DbSet<TimeAttendanceEditHistory> TimeAttendanceEditHistories { get; set; }
        public DbSet<ITCategory> ITCategories { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<ITForm> ITForms { get; set; }
        public DbSet<ITFormCategory> ITFormCategories { get; set; }
        public DbSet<AssignedTask> AssignTasks { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<ApplicationFormItem> ApplicationFormItems { get; set; }
        public DbSet<Delegations> Delegations { get; set; }
        public DbSet<TypeOverTime> TypeOverTimes { get; set; }
        public DbSet<OverTime> OverTimes { get; set; }

        public IDbConnection CreateConnection() => Database.GetDbConnection();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region SEED DATA

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin", Code = "SUPERADMIN" },
                new Role { Id = 2, Name = "HR", Code = "HR" },
                new Role { Id = 3, Name = "IT", Code = "IT" },
                new Role { Id = 4, Name = "Union", Code = "UNION" },
                new Role { Id = 5, Name = "User", Code = "USER" },
                new Role { Id = 6, Name = "GM", Code = "GM" },
                new Role { Id = 7, Name = "HR_Manager", Code = "HR_MANAGER" }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "time_keeping.mng_time_keeping", Group = "TIME_KEEPING" },
                new Permission { Id = 2, Name = "leave_request.hr_management_leave_request", Group = "LEAVE_REQUEST" },
                new Permission { Id = 3, Name = "memo_notification.create", Group = "MEMO_NOTIFICATION" }
            );

            modelBuilder.Entity<TypeLeave>().HasData(
                new TypeLeave { Id = 1, Code = "AL", NameE = "Annual Leave", Name = "Nghỉ Phép Năm" },
                new TypeLeave { Id = 2, Code = "NPL", NameE = "Unpaid Leave", Name = "Nghỉ Việc Cá Nhân" },
                new TypeLeave { Id = 3, Code = "MC", NameE = "Sick Leave", Name = "Nghỉ Ốm" },
                new TypeLeave { Id = 4, Code = "ML", NameE = "Wedding Leave", Name = "Nghỉ Cưới" },
                new TypeLeave { Id = 5, Code = "ACC", NameE = "Accident Leave", Name = "Nghỉ TNLĐ" },
                new TypeLeave { Id = 6, Code = "PL", NameE = "Paternity Leave", Name = "Nghỉ Vợ Sinh" },
                new TypeLeave { Id = 7, Code = "MAT", NameE = "Maternity Leave", Name = "Nghỉ Đẻ" },
                new TypeLeave { Id = 8, Code = "UL", NameE = "Compensatory Leave", Name = "Nghỉ Bù" },
                new TypeLeave { Id = 9, Code = "COMP", NameE = "Funeral Leave", Name = "Nghỉ Tang Lễ" },
                new TypeLeave { Id = 10, Code = "WO", NameE = "Working Outside", Name = "Làm Ở Ngoài" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    UserCode = "0",
                    Password = "$2a$12$GAJGsDDQUCEPfSqOLbPwmu5agSkYoaH6eUzLPJLRx2hnA89LSkiey", //123456
                    IsChangePassword = true,
                    IsActive = true,
                    Email = "superadmin@vsvn.com.vn",
                    Phone = "0999999999",
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
                new TimeLeave { Id = 1, Name = "Cả Ngày", NameE = "All Day" },
                new TimeLeave { Id = 2, Name = "Buổi sáng", NameE = "Morning" },
                new TimeLeave { Id = 3, Name = "Buổi chiều", NameE = "Afternoon" }
            );

            modelBuilder.Entity<RequestStatus>().HasData(
                new RequestStatus { Id = 1, Name = "Chờ duyệt", NameE = "PENDING" },
                new RequestStatus { Id = 2, Name = "Đang xử lý", NameE = "IN_PROCESS" },
                new RequestStatus { Id = 3, Name = "Hoàn thành", NameE = "COMPLETED" },
                new RequestStatus { Id = 4, Name = "Chờ HR", NameE = "WAIT_HR" },
                new RequestStatus { Id = 5, Name = "Từ chối", NameE = "REJECTED" },
                new RequestStatus { Id = 6, Name = "Duyệt cuối cùng", NameE = "FINAL_APPROVAL" },
                new RequestStatus { Id = 7, Name = "Đã giao", NameE = "ASSIGNED" }
            );

            modelBuilder.Entity<RequestType>().HasData(
                new RequestType { Id = 1, Name = "Nghỉ phép", NameE = "Leave request" },
                new RequestType { Id = 2, Name = "Thông báo", NameE = "Memo Notification" },
                new RequestType { Id = 3, Name = "Form IT", NameE = "Form IT" },
                new RequestType { Id = 4, Name = "Mua bán", NameE = "Purchase" }
            );

            modelBuilder.Entity<Unit>().HasData(
                new Unit { Id = 1, Name = "Company" },
                new Unit { Id = 2, Name = "Manage Department" },
                new Unit { Id = 3, Name = "Department" },
                new Unit { Id = 4, Name = "Team" },
                new Unit { Id = 5, Name = "GM" },
                new Unit { Id = 6, Name = "Manager" },
                new Unit { Id = 7, Name = "AM" }
            );

            modelBuilder.Entity<OrgUnit>().HasData(
                new OrgUnit { Id = 1, Name = "VS Industry VietNam", ParentOrgUnitId = null, UnitId = 1 },

                new OrgUnit { Id = 2, Name = "Business Development", ParentOrgUnitId = 1, UnitId = 2 }, 
                new OrgUnit { Id = 3, Name = "Finance & Admin", ParentOrgUnitId = 1, UnitId = 2 },
                new OrgUnit { Id = 4, Name = "Operations", ParentOrgUnitId = 1, UnitId = 2 },
                new OrgUnit { Id = 5, Name = "VS Technology", ParentOrgUnitId = 1, UnitId = 2 },

                //department
                new OrgUnit { Id = 6, Name = "General Manager", ParentOrgUnitId = 1, UnitId = 3 },
                new OrgUnit { Id = 7, Name = "Admin", ParentOrgUnitId = 1, UnitId = 3 },
                new OrgUnit { Id = 8, Name = "Production", ParentOrgUnitId = 4, UnitId = 3 },
                new OrgUnit { Id = 9, Name = "MIS", ParentOrgUnitId = 1, UnitId = 3 },
                new OrgUnit { Id = 10, Name = "HR", ParentOrgUnitId = 3, UnitId = 3 },
                new OrgUnit { Id = 11, Name = "Commercial", ParentOrgUnitId = 2, UnitId = 3 },
                new OrgUnit { Id = 12, Name = "Purchasing", ParentOrgUnitId = 1, UnitId = 3 },

                //team
                new OrgUnit { Id = 13, Name = "Production_VPSX", ParentOrgUnitId = 8, UnitId = 4 },
                new OrgUnit { Id = 14, Name = "12A_A", ParentOrgUnitId = 8, UnitId = 4 },
                new OrgUnit { Id = 15, Name = "12A_B", ParentOrgUnitId = 8, UnitId = 4 },
                new OrgUnit { Id = 16, Name = "12B_A", ParentOrgUnitId = 8, UnitId = 4 },
                new OrgUnit { Id = 17, Name = "12B_H", ParentOrgUnitId = 8, UnitId = 4 },
                new OrgUnit { Id = 18, Name = "Kỹ thuật A_AGH", ParentOrgUnitId = 8, UnitId = 4 },
                new OrgUnit { Id = 19, Name = "Kỹ thuật B_BCDEF", ParentOrgUnitId = 8, UnitId = 4 }
            );

            modelBuilder.Entity<OrgPosition>().HasData(
                new OrgPosition { Id = 1, PositionCode = "GD", Name = "General Director", OrgUnitId = 6, ParentOrgPositionId = null, UnitId = 5, IsStaff = true },
                new OrgPosition { Id = 2, PositionCode = "AM_GD", Name = "AM General Director", OrgUnitId = 6, ParentOrgPositionId = 1, UnitId = 5, IsStaff = true },
                new OrgPosition { Id = 3, PositionCode = "BDGM", Name = "BD General Manager", OrgUnitId = 6, ParentOrgPositionId = 1, UnitId = 5, IsStaff = true },
                new OrgPosition { Id = 4, PositionCode = "FGM", Name = "Finance General Manage", OrgUnitId = 6, ParentOrgPositionId = 1, UnitId = 5, IsStaff = true },
                new OrgPosition { Id = 5, PositionCode = "OGM", Name = "Operations General Manager", OrgUnitId = 6, ParentOrgPositionId = 1, UnitId = 5, IsStaff = true },
                new OrgPosition { Id = 6, PositionCode = "OM", Name = "Operations Manager", OrgUnitId = 6, ParentOrgPositionId = 1, UnitId = 5, IsStaff = true },

                new OrgPosition { Id = 7, PositionCode = "ADMIN-MGR", Name = "Manager Admin", OrgUnitId = 7, ParentOrgPositionId = 1, UnitId = 5, IsStaff = true },

                new OrgPosition { Id = 8, PositionCode = "MIS-MGR", Name = "Manager MIS/IT", OrgUnitId = 9, ParentOrgPositionId = 5, UnitId = 6, IsStaff = true },
                new OrgPosition { Id = 9, PositionCode = "MIS-Staff", Name = "Staff IT", OrgUnitId = 9, ParentOrgPositionId = 8, IsStaff = true },

                new OrgPosition { Id = 10, PositionCode = "COM-MGR", Name = "Manager Commercial", OrgUnitId = 11, ParentOrgPositionId = 3, UnitId = 6, IsStaff = true },
                new OrgPosition { Id = 11, PositionCode = "COM-AM", Name = "Assistant Manager Commercial", OrgUnitId = 11, ParentOrgPositionId = 10, IsStaff = true },
                new OrgPosition { Id = 12, PositionCode = "COM-Staff", Name = "Staff Commercial", OrgUnitId = 11, ParentOrgPositionId = 10, IsStaff = true },

                new OrgPosition { Id = 13, PositionCode = "HR-MGR", Name = "Manager HR", OrgUnitId = 10, ParentOrgPositionId = 1, UnitId = 6, IsStaff = true },
                new OrgPosition { Id = 14, PositionCode = "HR-AM", Name = "Assistant Manager HR", OrgUnitId = 10, ParentOrgPositionId = 7, IsStaff = true },
                new OrgPosition { Id = 15, PositionCode = "HR-Staff", Name = "Staff HR", OrgUnitId = 10, ParentOrgPositionId = 14, IsStaff = true },

                new OrgPosition { Id = 16, PositionCode = "PUR-MGR", Name = "Manager Purchasing", OrgUnitId = 12, ParentOrgPositionId = 5, UnitId = 6, IsStaff = true },
                new OrgPosition { Id = 17, PositionCode = "PUR-AM", Name = "Assistant Manager Purchasing", OrgUnitId = 12, ParentOrgPositionId = 16, IsStaff = true },
                new OrgPosition { Id = 18, PositionCode = "PUR-Staff", Name = "Staff Purchasing", OrgUnitId = 12, ParentOrgPositionId = 16, IsStaff = true },

                new OrgPosition { Id = 19, PositionCode = "PRD-MGR", Name = "Manager Production", OrgUnitId = 8, ParentOrgPositionId = 5, UnitId = 6, IsStaff = true },

                new OrgPosition { Id = 20, PositionCode = "PRD-SUP-AGH", Name = "Supervisor Tech A_AGH", OrgUnitId = 18, ParentOrgPositionId = 19 },
                new OrgPosition { Id = 21, PositionCode = "PRD-SUP-BBCDEF", Name = "Supervisor Tech B_BCDEF", OrgUnitId = 19, ParentOrgPositionId = 19 },
                new OrgPosition { Id = 22, PositionCode = "PRD-SUP-SHIFT-A", Name = "Supervisor Shift A", OrgUnitId = 14, ParentOrgPositionId = 19 },
                new OrgPosition { Id = 23, PositionCode = "PRD-SUP-SHIFT-B", Name = "Supervisor Shift B", OrgUnitId = 17, ParentOrgPositionId = 19 },

                new OrgPosition { Id = 24, PositionCode = "PRD-L-12AA", Name = "Leader 12A_A", OrgUnitId = 14, ParentOrgPositionId = 22 },
                new OrgPosition { Id = 25, PositionCode = "PRD-OP-12AA", Name = "Operator 12A_A", OrgUnitId = 14, ParentOrgPositionId = 24 },

                new OrgPosition { Id = 26, PositionCode = "PRD-L-12AB", Name = "Leader 12A_B", OrgUnitId = 15, ParentOrgPositionId = 22 },
                new OrgPosition { Id = 27, PositionCode = "PRD-OP-12AB", Name = "Operator 12A_B", OrgUnitId = 15, ParentOrgPositionId = 26 },

                new OrgPosition { Id = 28, PositionCode = "PRD-L-12BA", Name = "Leader 12B_A", OrgUnitId = 16, ParentOrgPositionId = 23 },
                new OrgPosition { Id = 29, PositionCode = "PRD-OP-12BA", Name = "Operator 12B_A", OrgUnitId = 16, ParentOrgPositionId = 28 },

                new OrgPosition { Id = 30, PositionCode = "PRD-L-12BH", Name = "Leader 12B_H", OrgUnitId = 17, ParentOrgPositionId = 23 },
                new OrgPosition { Id = 31, PositionCode = "PRD-OP-12BH", Name = "Operator 12B_H", OrgUnitId = 17, ParentOrgPositionId = 30 },

                new OrgPosition { Id = 32, PositionCode = "PRD-TECH-AAH", Name = "Technician A_AGH", OrgUnitId = 18, ParentOrgPositionId = 20 },
                new OrgPosition { Id = 33, PositionCode = "PRD-TECH-BCDEF", Name = "Technician B_BCDEF", OrgUnitId = 19, ParentOrgPositionId = 21 }
            );

            modelBuilder.Entity<Priority>().HasData(
                new Priority { Id = 1, Name = "Thấp", NameE = "Low" },
                new Priority { Id = 2, Name = "Trung bình", NameE = "Medium" },
                new Priority { Id = 3, Name = "Cao", NameE = "High" }
            );

            modelBuilder.Entity<ITCategory>().HasData(
                new ITCategory { Id = 1, Name = "Server Login Id", Code = "SERVER" },
                new ITCategory { Id = 2, Name = "Network device", Code = "NETWORK" },
                new ITCategory { Id = 3, Name = "Email", Code = "EMAIL" },
                new ITCategory { Id = 4, Name = "Software Installation", Code = "SOFTWARE" },
                new ITCategory { Id = 5, Name = "SAP Form", Code = "SAP" },
                new ITCategory { Id = 6, Name = "Other", Code = "OTHER" }
            );

            modelBuilder.Entity<CostCenter>().HasData(
                new CostCenter { Id = 1, Code = "V1013202", Description = "MIS" }
            );

            modelBuilder.Entity<TypeOverTime>().HasData(
                new TypeOverTime { Id = 1, Name = "Từ thứ 2 đến thứ 7", NameE = "Form monday to saturday" },
                new TypeOverTime { Id = 2, Name = "Chủ nhật", NameE = "Sunday"},
                new TypeOverTime { Id = 3, Name = "Ngày lễ", NameE = "Holiday"}
            );

            #endregion

            #region QUERY FILTER

            modelBuilder.Entity<ApplicationForm>().HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<ApplicationFormItem>().HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<HistoryApplicationForm>().HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<ITForm>().HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<Purchase>().HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<PurchaseDetail>().HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<MemoNotification>().HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<LeaveRequest>().HasQueryFilter(e => e.DeletedAt == null);

            #endregion

            #region USER - ROLE - PERMISSION - USER_CONFIG

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

            #endregion

            #region Leave_Request

            //type_leave
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.TypeLeave)
                .WithMany()
                .HasPrincipalKey(t => t.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //time_leavecls
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.TimeLeave)
                .WithMany()
                .HasPrincipalKey(t => t.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //org unit
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.OrgUnit)
                .WithMany()
                .HasPrincipalKey(o => o.Id)
                .HasForeignKey(lr => lr.DepartmentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //application_form_item
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ApplicationFormItem)
                .WithMany(afi => afi.LeaveRequests)
                .HasPrincipalKey(afi => afi.Id)
                .HasForeignKey(lr => lr.ApplicationFormItemId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region Memo_Notification - File - Attach file

            //file - attach_file
            modelBuilder.Entity<AttachFile>()
                .HasOne(a => a.File)
                .WithMany()
                .HasForeignKey(a => a.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            //memo - memo dept
            modelBuilder.Entity<MemoNotificationDepartment>()
                .HasOne(mnd => mnd.MemoNotifications)
                .WithMany(mn => mn.MemoNotificationDepartments)
                .HasPrincipalKey(mn => mn.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            //memo department - org unit
            modelBuilder.Entity<MemoNotificationDepartment>()
                .HasOne(mnd => mnd.OrgUnit)
                .WithMany()
                .HasForeignKey(mnd => mnd.DepartmentId)
                .HasPrincipalKey(ou => ou.Id)
                .OnDelete(DeleteBehavior.NoAction);

            //memo - org unit
            modelBuilder.Entity<MemoNotification>()
                .HasOne(mnd => mnd.OrgUnit)
                .WithMany()
                .HasForeignKey(mnd => mnd.DepartmentId)
                .HasPrincipalKey(ou => ou.Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MemoNotification>()
                .HasOne(lr => lr.ApplicationFormItem)
                .WithMany(afi => afi.MemoNotifications)
                .HasPrincipalKey(afi => afi.Id)
                .HasForeignKey(lr => lr.ApplicationFormItemId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region IT_Form

            modelBuilder.Entity<ITForm>()
                .HasOne(it => it.Priority)
                .WithMany()
                .HasPrincipalKey(t => t.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ITForm>()
                .HasOne(lr => lr.OrgUnit)
                .WithMany()
                .HasPrincipalKey(o => o.Id)
                .HasForeignKey(lr => lr.DepartmentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ITForm>()
                .HasOne(lr => lr.ApplicationFormItem)
                .WithMany(afi => afi.ITForms)
                .HasPrincipalKey(afi => afi.Id)
                .HasForeignKey(lr => lr.ApplicationFormItemId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region Purchasing

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.OrgUnit)
                .WithMany()
                .HasForeignKey(p => p.DepartmentId)
                .HasPrincipalKey(ou => ou.Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Purchase>()
                .HasMany(p => p.PurchaseDetails)
                .WithOne()
                .HasForeignKey(pl => pl.PurchaseId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PurchaseDetail>()
                .HasOne(pl => pl.CostCenter)
                .WithMany()
                .HasForeignKey(pl => pl.CostCenterId)
                .HasPrincipalKey(cc => cc.Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Purchase>()
                .HasOne(lr => lr.ApplicationFormItem)
                .WithMany(afi => afi.Purchases)
                .HasPrincipalKey(afi => afi.Id)
                .HasForeignKey(lr => lr.ApplicationFormItemId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region OrgUnit

            modelBuilder.Entity<OrgUnit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.ParentOrgUnit)
                      .WithMany(e => e.Children)
                      .HasForeignKey(e => e.ParentOrgUnitId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region OrgPosition

            modelBuilder.Entity<OrgPosition>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.ParentOrgPosition)
                      .WithMany()
                      .HasForeignKey(e => e.ParentOrgPositionId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrgPosition>()
                .HasOne(op => op.Unit)
                .WithMany()
                .HasForeignKey(op => op.UnitId)
                .HasPrincipalKey(unit => unit.Id)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region ApplicationForm - History_Application_Form

            modelBuilder.Entity<ApplicationForm>()
                .HasMany(af => af.ApplicationFormItems)
                .WithOne(afi => afi.ApplicationForm)
                .HasForeignKey(afi => afi.ApplicationFormId)
                .HasPrincipalKey(af => af.Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ApplicationForm>()
                .HasOne(af => af.OrgUnit)
                .WithMany()
                .HasForeignKey(af => af.DepartmentId)
                .HasPrincipalKey(ou => ou.Id)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            modelBuilder.Entity<HistoryApplicationForm>()
                .HasOne(h => h.ApplicationForm)
                .WithMany(a => a.HistoryApplicationForms)
                .HasForeignKey(h => h.ApplicationFormId)
                .HasPrincipalKey(a => a.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region Over Time

            modelBuilder.Entity<OverTime>()
                .HasOne(ot => ot.TypeOverTime)
                .WithMany()
                .HasPrincipalKey(t => t.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OverTime>()
                .HasOne(ot => ot.ApplicationFormItem)
                .WithMany(afi => afi.OverTimes)
                .HasPrincipalKey(afi => afi.Id)
                .HasForeignKey(ot => ot.ApplicationFormItemId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion
        }
    }
}
