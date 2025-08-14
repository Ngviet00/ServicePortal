using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "approval_flows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestTypeId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    Step = table.Column<int>(type: "int", nullable: true),
                    FromPositionId = table.Column<int>(type: "int", nullable: true),
                    PositonContext = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToPositionId = table.Column<int>(type: "int", nullable: true),
                    ToSpecificUserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_flows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "delegations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionId = table.Column<int>(type: "int", nullable: true),
                    FromUserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToUserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ToDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsPermanent = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delegations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Group = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "request_statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_request_statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "request_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_request_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "system_configs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfigValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "time_attendance_edit_histories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserCodeUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSentToHR = table.Column<bool>(type: "bit", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_attendance_edit_histories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "time_leaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_leaves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "type_leaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_leaves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_mng_org_unit_id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: true),
                    ManagementType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_mng_org_unit_id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsChangePassword = table.Column<byte>(type: "tinyint", nullable: true),
                    IsActive = table.Column<byte>(type: "tinyint", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.UniqueConstraint("AK_users_UserCode", x => x.UserCode);
                });

            migrationBuilder.CreateTable(
                name: "attach_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attach_files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    PermissionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "org_units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentOrgUnitId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org_units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PermissionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgUnitId = table.Column<int>(type: "int", nullable: true),
                    ParentPositionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "application_forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCodeRequestor = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserNameRequestor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestTypeId = table.Column<int>(type: "int", nullable: true),
                    RequestStatusId = table.Column<int>(type: "int", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "history_application_forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserNameApproval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserCodeApproval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_history_application_forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "leave_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserCodeRequestor = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserNameRequestor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ToDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TypeLeaveId = table.Column<int>(type: "int", nullable: true),
                    TimeLeaveId = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HaveSalary = table.Column<byte>(type: "tinyint", nullable: true),
                    UserCodeCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "memo_notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ToDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UserCodeCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    ApplyAllDepartment = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OrgUnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memo_notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "memo_notification_departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemoNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memo_notification_departments", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "Id", "Group", "Name" },
                values: new object[,]
                {
                    { 1, "TIME_KEEPING", "time_keeping.mng_time_keeping" },
                    { 2, "LEAVE_REQUEST", "leave_request.create_multiple_leave_request" },
                    { 3, "LEAVE_REQUEST", "leave_request.hr_management_leave_request" },
                    { 4, "MEMO_NOTIFICATION", "memo_notification.create" }
                });

            migrationBuilder.InsertData(
                table: "request_statuses",
                columns: new[] { "Id", "Name", "NameE" },
                values: new object[,]
                {
                    { 1, "Chờ duyệt", "PENDING" },
                    { 2, "Đang xử lý", "IN_PROCESS" },
                    { 3, "Hoàn thành", "COMPLETED" },
                    { 4, "Chờ HR", "WAIT_HR" },
                    { 5, "Từ chối", "REJECTED" },
                    { 6, "Duyệt cuối cùng", "FINAL_APPROVAL" },
                    { 7, "Đã giao", "ASSIGNED" }
                });

            migrationBuilder.InsertData(
                table: "request_types",
                columns: new[] { "Id", "Name", "NameE" },
                values: new object[,]
                {
                    { 1, "Nghỉ phép", "Leave request" },
                    { 2, "Chấm công", "Time Keeping" },
                    { 3, "Thông báo", "Memo Notification" },
                    { 4, "Form IT", "Form IT" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "SUPERADMIN", "SuperAdmin" },
                    { 2, "HR", "HR" },
                    { 3, "IT", "IT" },
                    { 4, "UNION", "Union" },
                    { 5, "USER", "User" },
                    { 6, "GM", "GM" },
                    { 7, "HR_MANAGER", "HR_Manager" }
                });

            migrationBuilder.InsertData(
                table: "time_leaves",
                columns: new[] { "Id", "Name", "NameE" },
                values: new object[,]
                {
                    { 1, "Cả Ngày", "All Day" },
                    { 2, "Buổi sáng", "Morning" },
                    { 3, "Buổi chiều", "Afternoon" }
                });

            migrationBuilder.InsertData(
                table: "type_leaves",
                columns: new[] { "Id", "Code", "Name", "NameE" },
                values: new object[,]
                {
                    { 1, "AL", "Nghỉ Phép Năm", "Annual Leave" },
                    { 2, "NPL", "Nghỉ Việc Cá Nhân", "Unpaid Leave" },
                    { 3, "MC", "Nghỉ Ốm", "Sick Leave" },
                    { 4, "ML", "Nghỉ Cưới", "Wedding Leave" },
                    { 5, "ACC", "Nghỉ TNLĐ", "Accident Leave" },
                    { 6, "PL", "Nghỉ vợ sinh", "Paternity Leave" },
                    { 7, "MAT", "Nghỉ đẻ", "Maternity Leave" },
                    { 8, "UL", "Nghỉ bù", "Compensatory Leave" },
                    { 9, "COMP", "Nghỉ tang lễ", "Funeral Leave" },
                    { 10, "Wo", "Làm ở ngoài", "Working Outside" }
                });

            migrationBuilder.InsertData(
                table: "units",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Company" },
                    { 2, "Manage Department" },
                    { 3, "Department" },
                    { 4, "Team" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "DateOfBirth", "DeletedAt", "Email", "IsActive", "IsChangePassword", "Password", "Phone", "UserCode" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), null, null, "superadmin@vsvn.com.vn", (byte)1, (byte)1, "$2a$12$GAJGsDDQUCEPfSqOLbPwmu5agSkYoaH6eUzLPJLRx2hnA89LSkiey", "0999999999", "0" });

            migrationBuilder.InsertData(
                table: "org_units",
                columns: new[] { "Id", "Name", "ParentOrgUnitId", "UnitId" },
                values: new object[,]
                {
                    { 1, "VS Industry VietNam", null, 1 },
                    { 2, "Business Development", 1, 2 },
                    { 3, "Finance & Admin", 1, 2 },
                    { 4, "Operations", 1, 2 },
                    { 5, "VS Technology", 1, 2 },
                    { 6, "General Manager", 1, 3 },
                    { 7, "Production", 4, 3 },
                    { 8, "MIS", 1, 3 },
                    { 9, "HR", 3, 3 },
                    { 10, "Commercial", 4, 3 },
                    { 14, "12A_A", 6, 4 },
                    { 15, "12A_B", 6, 4 },
                    { 16, "12B_A", 6, 4 },
                    { 17, "12B_H", 6, 4 },
                    { 18, "Kỹ thuật A_AGH", 6, 4 },
                    { 19, "Kỹ thuật B_BCDEF", 6, 4 }
                });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "Id", "RoleId", "UserCode" },
                values: new object[] { 1, 1, "0" });

            migrationBuilder.InsertData(
                table: "positions",
                columns: new[] { "Id", "Name", "OrgUnitId", "ParentPositionId", "PositionCode" },
                values: new object[,]
                {
                    { 1, "General Director", 6, null, "GD" },
                    { 2, "AM General Director", 6, 1, "AM_GD" },
                    { 3, "BD General Manager", 6, 1, "BDGM" },
                    { 4, "Finance General Manage", 6, 1, "FGM" },
                    { 5, "Operations General Manager", 6, 1, "OGM" },
                    { 6, "Operations Manager", 6, 1, "OM" },
                    { 7, "Manager MIS/IT", 8, null, "MIS-MGR" },
                    { 8, "Staff IT", 8, 7, "MIS-Staff" },
                    { 9, "Manager Commercial", 10, null, "COM-MGR" },
                    { 10, "AM Commercial", 10, 9, "COM-AM" },
                    { 11, "Staff Commercial", 10, 10, "COM-Staff" },
                    { 12, "Manager HR", 9, null, "HR-MGR" },
                    { 13, "AM HR", 9, 12, "HR-AM" },
                    { 14, "Staff HR", 9, 13, "HR-Staff" },
                    { 15, "Manager Production", 7, null, "PRD-MGR" },
                    { 16, "Supervisor A_AGH", 18, 15, "PRD-S-AGH" },
                    { 17, "Supervisor B_BCDEF", 19, 15, "PRD-S-BBCDEF" },
                    { 18, "Supervisor Shift A", 14, 15, "PRD-S-SA" },
                    { 19, "Supervisor Shift B", 17, 15, "PRD-S-SB" },
                    { 20, "12A_A Leader", 14, 18, "PRD-12AA-L" },
                    { 21, "12A_A Operator", 14, 18, "PRD-12AA-OP" },
                    { 22, "12B_A Leader", 16, 19, "PRD-12BA-L" },
                    { 23, "12B_A Operator", 16, 19, "PRD-12BA-OP" },
                    { 24, "Technician A_AGH", 18, 16, "PRD-T-AAH" },
                    { 25, "Technician B_BCDEF", 19, 17, "PRD-T-BCDEF" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_PositionId",
                table: "application_forms",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_RequestStatusId",
                table: "application_forms",
                column: "RequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_RequestTypeId",
                table: "application_forms",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_UserCodeRequestor",
                table: "application_forms",
                column: "UserCodeRequestor");

            migrationBuilder.CreateIndex(
                name: "IX_approval_flows_FromPositionId",
                table: "approval_flows",
                column: "FromPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_attach_files_EntityType_EntityId",
                table: "attach_files",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_attach_files_FileId",
                table: "attach_files",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_files_Id",
                table: "files",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_history_application_forms_ApplicationFormId",
                table: "history_application_forms",
                column: "ApplicationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_ApplicationFormId",
                table: "leave_requests",
                column: "ApplicationFormId",
                unique: true,
                filter: "[ApplicationFormId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_DepartmentId",
                table: "leave_requests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_Id_UserCodeRequestor",
                table: "leave_requests",
                columns: new[] { "Id", "UserCodeRequestor" });

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_TimeLeaveId",
                table: "leave_requests",
                column: "TimeLeaveId");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_TypeLeaveId",
                table: "leave_requests",
                column: "TypeLeaveId");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_UserCodeRequestor",
                table: "leave_requests",
                column: "UserCodeRequestor");

            migrationBuilder.CreateIndex(
                name: "IX_memo_notification_departments_DepartmentId",
                table: "memo_notification_departments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_memo_notification_departments_MemoNotificationId_DepartmentId",
                table: "memo_notification_departments",
                columns: new[] { "MemoNotificationId", "DepartmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_ApplicationFormId",
                table: "memo_notifications",
                column: "ApplicationFormId",
                unique: true,
                filter: "[ApplicationFormId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_OrgUnitId",
                table: "memo_notifications",
                column: "OrgUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_org_units_UnitId",
                table: "org_units",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Id",
                table: "permissions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_positions_OrgUnitId",
                table: "positions",
                column: "OrgUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_ExpiresAt",
                table: "refresh_tokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_Token",
                table: "refresh_tokens",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserCode",
                table: "refresh_tokens",
                column: "UserCode");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_PermissionId",
                table: "role_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_RoleId_PermissionId",
                table: "role_permissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_roles_Id",
                table: "roles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_user_configs_UserCode",
                table: "user_configs",
                column: "UserCode");

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_PermissionId",
                table: "user_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_UserCode_PermissionId",
                table: "user_permissions",
                columns: new[] { "UserCode", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_UserCode_RoleId",
                table: "user_roles",
                columns: new[] { "UserCode", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_users_UserCode",
                table: "users",
                column: "UserCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "approval_flows");

            migrationBuilder.DropTable(
                name: "attach_files");

            migrationBuilder.DropTable(
                name: "delegations");

            migrationBuilder.DropTable(
                name: "history_application_forms");

            migrationBuilder.DropTable(
                name: "leave_requests");

            migrationBuilder.DropTable(
                name: "memo_notification_departments");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "system_configs");

            migrationBuilder.DropTable(
                name: "time_attendance_edit_histories");

            migrationBuilder.DropTable(
                name: "user_configs");

            migrationBuilder.DropTable(
                name: "user_mng_org_unit_id");

            migrationBuilder.DropTable(
                name: "user_permissions");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "time_leaves");

            migrationBuilder.DropTable(
                name: "type_leaves");

            migrationBuilder.DropTable(
                name: "memo_notifications");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "application_forms");

            migrationBuilder.DropTable(
                name: "positions");

            migrationBuilder.DropTable(
                name: "request_statuses");

            migrationBuilder.DropTable(
                name: "request_types");

            migrationBuilder.DropTable(
                name: "org_units");

            migrationBuilder.DropTable(
                name: "units");
        }
    }
}
