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
                    FromOrgPositionId = table.Column<int>(type: "int", nullable: true),
                    PositonContext = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToOrgPositionId = table.Column<int>(type: "int", nullable: true),
                    ToSpecificUserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_flows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cost_centers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cost_centers", x => x.Id);
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
                name: "it_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_it_categories", x => x.Id);
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
                name: "priorities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priorities", x => x.Id);
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
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
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
                    OrgUnitId = table.Column<int>(type: "int", nullable: true),
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
                name: "application_forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestTypeId = table.Column<int>(type: "int", nullable: true),
                    RequestStatusId = table.Column<int>(type: "int", nullable: true),
                    OrgPositionId = table.Column<int>(type: "int", nullable: true),
                    UserCodeCreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Step = table.Column<int>(type: "int", nullable: true),
                    MetaData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_forms", x => x.Id);
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
                name: "application_form_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_form_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "assigned_tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assigned_tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "history_application_forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_history_application_forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "it_forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriorityId = table.Column<int>(type: "int", nullable: true),
                    NoteManagerIT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RequiredCompletionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TargetCompletionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActualCompletionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_it_forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "memo_notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ToDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    ApplyAllDepartment = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memo_notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "org_positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgUnitId = table.Column<int>(type: "int", nullable: true),
                    ParentOrgPositionId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    IsStaff = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org_positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "purchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    RequestedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "leave_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationFormItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ToDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TypeLeaveId = table.Column<int>(type: "int", nullable: true),
                    TimeLeaveId = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    HaveSalary = table.Column<byte>(type: "tinyint", nullable: true),
                    NoteOfHR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "it_form_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ITFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ITCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_it_form_categories", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "purchase_details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitMeasurement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_details", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "cost_centers",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[] { 1, "V1013202", "MIS" });

            migrationBuilder.InsertData(
                table: "it_categories",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "SERVER", "Server Login Id" },
                    { 2, "NETWORK", "Network device" },
                    { 3, "EMAIL", "Email" },
                    { 4, "SOFTWARE", "Software Installation" },
                    { 5, "ERP", "ERP Login Id" },
                    { 6, "OTHER", "Other" }
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
                table: "priorities",
                columns: new[] { "Id", "Name", "NameE" },
                values: new object[,]
                {
                    { 1, "Thấp", "Low" },
                    { 2, "Trung bình", "Medium" },
                    { 3, "Cao", "High" }
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
                values: new object[] { 1, "VS Industry VietNam", null, 1 });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "Id", "RoleId", "UserCode" },
                values: new object[] { 1, 1, "0" });

            migrationBuilder.InsertData(
                table: "org_units",
                columns: new[] { "Id", "Name", "ParentOrgUnitId", "UnitId" },
                values: new object[,]
                {
                    { 2, "Business Development", 1, 2 },
                    { 3, "Finance & Admin", 1, 2 },
                    { 4, "Operations", 1, 2 },
                    { 5, "VS Technology", 1, 2 },
                    { 6, "General Manager", 1, 3 },
                    { 8, "MIS", 1, 3 }
                });

            migrationBuilder.InsertData(
                table: "org_positions",
                columns: new[] { "Id", "IsStaff", "Name", "OrgUnitId", "ParentOrgPositionId", "PositionCode", "UnitId" },
                values: new object[,]
                {
                    { 1, null, "General Director", 6, null, "GD", null },
                    { 7, null, "Manager MIS/IT", 8, null, "MIS-MGR", null }
                });

            migrationBuilder.InsertData(
                table: "org_units",
                columns: new[] { "Id", "Name", "ParentOrgUnitId", "UnitId" },
                values: new object[,]
                {
                    { 7, "Production", 4, 3 },
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
                table: "org_positions",
                columns: new[] { "Id", "IsStaff", "Name", "OrgUnitId", "ParentOrgPositionId", "PositionCode", "UnitId" },
                values: new object[,]
                {
                    { 2, null, "AM General Director", 6, 1, "AM_GD", null },
                    { 3, null, "BD General Manager", 6, 1, "BDGM", null },
                    { 4, null, "Finance General Manage", 6, 1, "FGM", null },
                    { 5, null, "Operations General Manager", 6, 1, "OGM", null },
                    { 6, null, "Operations Manager", 6, 1, "OM", null },
                    { 8, null, "Staff IT", 8, 7, "MIS-Staff", null },
                    { 9, null, "Manager Commercial", 10, null, "COM-MGR", null },
                    { 12, null, "Manager HR", 9, null, "HR-MGR", null },
                    { 15, null, "Manager Production", 7, null, "PRD-MGR", null },
                    { 23, null, "12B_A Operator", 16, 23, "PRD-12BA-OP", null },
                    { 10, null, "AM Commercial", 10, 9, "COM-AM", null },
                    { 13, null, "AM HR", 9, 12, "HR-AM", null },
                    { 16, null, "Supervisor A_AGH", 18, 15, "PRD-S-AGH", null },
                    { 17, null, "Supervisor B_BCDEF", 19, 15, "PRD-S-BBCDEF", null },
                    { 18, null, "Supervisor Shift A", 14, 15, "PRD-S-SA", null },
                    { 19, null, "Supervisor Shift B", 17, 15, "PRD-S-SB", null },
                    { 11, null, "Staff Commercial", 10, 10, "COM-Staff", null },
                    { 14, null, "Staff HR", 9, 13, "HR-Staff", null },
                    { 20, null, "12A_A Leader", 14, 18, "PRD-12AA-L", null },
                    { 22, null, "12B_A Leader", 16, 19, "PRD-12BA-L", null },
                    { 24, null, "Technician A_AGH", 18, 16, "PRD-T-AAH", null },
                    { 25, null, "Technician B_BCDEF", 19, 17, "PRD-T-BCDEF", null },
                    { 21, null, "12A_A Operator", 14, 20, "PRD-12AA-OP", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_form_items_ApplicationFormId",
                table: "application_form_items",
                column: "ApplicationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_OrgPositionId",
                table: "application_forms",
                column: "OrgPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_RequestStatusId",
                table: "application_forms",
                column: "RequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_RequestTypeId",
                table: "application_forms",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_UserCodeCreatedBy",
                table: "application_forms",
                column: "UserCodeCreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_approval_flows_FromOrgPositionId_ToOrgPositionId",
                table: "approval_flows",
                columns: new[] { "FromOrgPositionId", "ToOrgPositionId" });

            migrationBuilder.CreateIndex(
                name: "IX_assigned_tasks_ApplicationFormId",
                table: "assigned_tasks",
                column: "ApplicationFormId");

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
                name: "IX_it_form_categories_ITCategoryId",
                table: "it_form_categories",
                column: "ITCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_it_form_categories_ITFormId",
                table: "it_form_categories",
                column: "ITFormId");

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_ApplicationFormId",
                table: "it_forms",
                column: "ApplicationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_DepartmentId",
                table: "it_forms",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_PriorityId",
                table: "it_forms",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_ApplicationFormItemId",
                table: "leave_requests",
                column: "ApplicationFormItemId");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_DepartmentId",
                table: "leave_requests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_Id",
                table: "leave_requests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_TimeLeaveId",
                table: "leave_requests",
                column: "TimeLeaveId");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_TypeLeaveId",
                table: "leave_requests",
                column: "TypeLeaveId");

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
                column: "ApplicationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_DepartmentId",
                table: "memo_notifications",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_org_positions_OrgUnitId",
                table: "org_positions",
                column: "OrgUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_org_positions_ParentOrgPositionId",
                table: "org_positions",
                column: "ParentOrgPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_org_positions_UnitId",
                table: "org_positions",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_org_units_ParentOrgUnitId",
                table: "org_units",
                column: "ParentOrgUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_org_units_UnitId",
                table: "org_units",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Id",
                table: "permissions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_details_CostCenterId",
                table: "purchase_details",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_details_PurchaseId",
                table: "purchase_details",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_ApplicationFormId",
                table: "purchases",
                column: "ApplicationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_DepartmentId",
                table: "purchases",
                column: "DepartmentId");

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
                name: "assigned_tasks");

            migrationBuilder.DropTable(
                name: "attach_files");

            migrationBuilder.DropTable(
                name: "delegations");

            migrationBuilder.DropTable(
                name: "history_application_forms");

            migrationBuilder.DropTable(
                name: "it_form_categories");

            migrationBuilder.DropTable(
                name: "leave_requests");

            migrationBuilder.DropTable(
                name: "memo_notification_departments");

            migrationBuilder.DropTable(
                name: "org_positions");

            migrationBuilder.DropTable(
                name: "purchase_details");

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
                name: "it_categories");

            migrationBuilder.DropTable(
                name: "it_forms");

            migrationBuilder.DropTable(
                name: "application_form_items");

            migrationBuilder.DropTable(
                name: "time_leaves");

            migrationBuilder.DropTable(
                name: "type_leaves");

            migrationBuilder.DropTable(
                name: "memo_notifications");

            migrationBuilder.DropTable(
                name: "cost_centers");

            migrationBuilder.DropTable(
                name: "purchases");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "priorities");

            migrationBuilder.DropTable(
                name: "application_forms");

            migrationBuilder.DropTable(
                name: "org_units");

            migrationBuilder.DropTable(
                name: "request_statuses");

            migrationBuilder.DropTable(
                name: "request_types");

            migrationBuilder.DropTable(
                name: "units");
        }
    }
}
