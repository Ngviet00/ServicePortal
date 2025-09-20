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
                    RequestTypeId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Step = table.Column<int>(type: "int", nullable: false),
                    FromOrgPositionId = table.Column<int>(type: "int", nullable: false),
                    PositonContext = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ToOrgPositionId = table.Column<int>(type: "int", nullable: false),
                    ToSpecificUserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
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
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Group = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
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
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datetime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CurrentValue = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UserCodeUpdated = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsSentToHR = table.Column<bool>(type: "bit", nullable: false),
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
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
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
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
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
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
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
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    OrgUnitId = table.Column<int>(type: "int", nullable: false),
                    ManagementType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsChangePassword = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<long>(type: "bigint", nullable: false)
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
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentOrgUnitId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org_units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_configs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true)
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
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
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
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "application_forms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestTypeId = table.Column<int>(type: "int", nullable: false),
                    RequestStatusId = table.Column<int>(type: "int", nullable: false),
                    OrgPositionId = table.Column<int>(type: "int", nullable: false),
                    UserCodeCreatedForm = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UserNameCreatedForm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Step = table.Column<int>(type: "int", nullable: false),
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
                name: "org_positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrgUnitId = table.Column<int>(type: "int", nullable: false),
                    ParentOrgPositionId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    IsStaff = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org_positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "application_form_items",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormId = table.Column<long>(type: "bigint", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RejectedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_form_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "assigned_tasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormId = table.Column<long>(type: "bigint", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assigned_tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "history_application_forms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormId = table.Column<long>(type: "bigint", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UserCodeAction = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UserNameAction = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormItemId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriorityId = table.Column<int>(type: "int", nullable: false),
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
                name: "leave_requests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormItemId = table.Column<long>(type: "bigint", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FromDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ToDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TypeLeaveId = table.Column<int>(type: "int", nullable: false),
                    TimeLeaveId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    HaveSalary = table.Column<bool>(type: "bit", nullable: false),
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
                name: "memo_notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormItemId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ToDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    ApplyAllDepartment = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memo_notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "purchases",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormItemId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
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
                name: "it_form_categories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ITFormId = table.Column<long>(type: "bigint", nullable: false),
                    ITCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_it_form_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "memo_notification_departments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemoNotificationId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memo_notification_departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "purchase_details",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<long>(type: "bigint", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitMeasurement = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    RequiredDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CostCenterId = table.Column<int>(type: "int", nullable: false),
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
                    { 5, "SAP", "SAP Form" },
                    { 6, "OTHER", "Other" }
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "Id", "Group", "Name" },
                values: new object[,]
                {
                    { 1, "TIME_KEEPING", "time_keeping.mng_time_keeping" },
                    { 2, "LEAVE_REQUEST", "leave_request.hr_management_leave_request" },
                    { 3, "MEMO_NOTIFICATION", "memo_notification.create" }
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
                    { 2, "Thông báo", "Memo Notification" },
                    { 3, "Form IT", "Form IT" },
                    { 4, "Mua bán", "Purchase" }
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
                    { 6, "PL", "Nghỉ Vợ Sinh", "Paternity Leave" },
                    { 7, "MAT", "Nghỉ Đẻ", "Maternity Leave" },
                    { 8, "UL", "Nghỉ Bù", "Compensatory Leave" },
                    { 9, "COMP", "Nghỉ Tang Lễ", "Funeral Leave" },
                    { 10, "WO", "Làm Ở Ngoài", "Working Outside" }
                });

            migrationBuilder.InsertData(
                table: "units",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Company" },
                    { 2, "Manage Department" },
                    { 3, "Department" },
                    { 4, "Team" },
                    { 5, "GM" },
                    { 6, "Manager" },
                    { 7, "AM" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "DateOfBirth", "DeletedAt", "Email", "IsActive", "IsChangePassword", "Password", "Phone", "UserCode" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), null, null, "superadmin@vsvn.com.vn", true, true, "$2a$12$GAJGsDDQUCEPfSqOLbPwmu5agSkYoaH6eUzLPJLRx2hnA89LSkiey", "0999999999", "0" });

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
                    { 7, "Admin", 1, 3 },
                    { 9, "MIS", 1, 3 },
                    { 12, "Purchasing", 1, 3 }
                });

            migrationBuilder.InsertData(
                table: "org_positions",
                columns: new[] { "Id", "IsStaff", "Name", "OrgUnitId", "ParentOrgPositionId", "PositionCode", "UnitId" },
                values: new object[] { 1, true, "General Director", 6, null, "GD", 5 });

            migrationBuilder.InsertData(
                table: "org_units",
                columns: new[] { "Id", "Name", "ParentOrgUnitId", "UnitId" },
                values: new object[,]
                {
                    { 8, "Production", 4, 3 },
                    { 10, "HR", 3, 3 },
                    { 11, "Commercial", 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "org_positions",
                columns: new[] { "Id", "IsStaff", "Name", "OrgUnitId", "ParentOrgPositionId", "PositionCode", "UnitId" },
                values: new object[,]
                {
                    { 2, true, "AM General Director", 6, 1, "AM_GD", 5 },
                    { 3, true, "BD General Manager", 6, 1, "BDGM", 5 },
                    { 4, true, "Finance General Manage", 6, 1, "FGM", 5 },
                    { 5, true, "Operations General Manager", 6, 1, "OGM", 5 },
                    { 6, true, "Operations Manager", 6, 1, "OM", 5 },
                    { 7, true, "Manager Admin", 7, 1, "ADMIN-MGR", 5 },
                    { 13, true, "Manager HR", 10, 1, "HR-MGR", 6 }
                });

            migrationBuilder.InsertData(
                table: "org_units",
                columns: new[] { "Id", "Name", "ParentOrgUnitId", "UnitId" },
                values: new object[,]
                {
                    { 13, "Production_VPSX", 8, 4 },
                    { 14, "12A_A", 8, 4 },
                    { 15, "12A_B", 8, 4 },
                    { 16, "12B_A", 8, 4 },
                    { 17, "12B_H", 8, 4 },
                    { 18, "Kỹ thuật A_AGH", 8, 4 },
                    { 19, "Kỹ thuật B_BCDEF", 8, 4 }
                });

            migrationBuilder.InsertData(
                table: "org_positions",
                columns: new[] { "Id", "IsStaff", "Name", "OrgUnitId", "ParentOrgPositionId", "PositionCode", "UnitId" },
                values: new object[,]
                {
                    { 8, true, "Manager MIS/IT", 9, 5, "MIS-MGR", 6 },
                    { 10, true, "Manager Commercial", 11, 3, "COM-MGR", 6 },
                    { 14, true, "Assistant Manager HR", 10, 7, "HR-AM", 0 },
                    { 16, true, "Manager Purchasing", 12, 5, "PUR-MGR", 6 },
                    { 19, true, "Manager Production", 8, 5, "PRD-MGR", 6 },
                    { 9, true, "Staff IT", 9, 8, "MIS-Staff", 0 },
                    { 11, true, "Assistant Manager Commercial", 11, 10, "COM-AM", 0 },
                    { 12, true, "Staff Commercial", 11, 10, "COM-Staff", 0 },
                    { 15, true, "Staff HR", 10, 14, "HR-Staff", 0 },
                    { 17, true, "Assistant Manager Purchasing", 12, 16, "PUR-AM", 0 },
                    { 18, true, "Staff Purchasing", 12, 16, "PUR-Staff", 0 },
                    { 20, false, "Supervisor Tech A_AGH", 18, 19, "PRD-SUP-AGH", 0 },
                    { 21, false, "Supervisor Tech B_BCDEF", 19, 19, "PRD-SUP-BBCDEF", 0 },
                    { 22, false, "Supervisor Shift A", 14, 19, "PRD-SUP-SHIFT-A", 0 },
                    { 23, false, "Supervisor Shift B", 17, 19, "PRD-SUP-SHIFT-B", 0 },
                    { 24, false, "Leader 12A_A", 14, 22, "PRD-L-12AA", 0 },
                    { 26, false, "Leader 12A_B", 15, 22, "PRD-L-12AB", 0 },
                    { 28, false, "Leader 12B_A", 16, 23, "PRD-L-12BA", 0 },
                    { 30, false, "Leader 12B_H", 17, 23, "PRD-L-12BH", 0 },
                    { 32, false, "Technician A_AGH", 18, 20, "PRD-TECH-AAH", 0 },
                    { 33, false, "Technician B_BCDEF", 19, 21, "PRD-TECH-BCDEF", 0 },
                    { 25, false, "Operator 12A_A", 14, 24, "PRD-OP-12AA", 0 },
                    { 27, false, "Operator 12A_B", 15, 26, "PRD-OP-12AB", 0 },
                    { 29, false, "Operator 12B_A", 16, 28, "PRD-OP-12BA", 0 },
                    { 31, false, "Operator 12B_H", 17, 30, "PRD-OP-12BH", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_form_items_ApplicationFormId",
                table: "application_form_items",
                column: "ApplicationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_application_form_items_Id",
                table: "application_form_items",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_Code",
                table: "application_forms",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_Id",
                table: "application_forms",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_OrgPositionId",
                table: "application_forms",
                column: "OrgPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_assigned_tasks_ApplicationFormId",
                table: "assigned_tasks",
                column: "ApplicationFormId");

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
                name: "IX_it_forms_ApplicationFormItemId",
                table: "it_forms",
                column: "ApplicationFormItemId");

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
                name: "IX_memo_notification_departments_DepartmentId",
                table: "memo_notification_departments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_memo_notification_departments_MemoNotificationId_DepartmentId",
                table: "memo_notification_departments",
                columns: new[] { "MemoNotificationId", "DepartmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_ApplicationFormItemId",
                table: "memo_notifications",
                column: "ApplicationFormItemId");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_details_PurchaseId",
                table: "purchase_details",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_ApplicationFormItemId",
                table: "purchases",
                column: "ApplicationFormItemId");

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
                name: "application_form_items");

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
