using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "approval_actions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproverUserCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApproverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_actions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "approval_flows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromPosition = table.Column<int>(type: "int", nullable: true),
                    ToPosition = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    StepOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_flows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "approval_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterUserCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RequestType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentPositionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "leave_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterUserCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserCodeWriteLeave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ToDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TypeLeave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeLeave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HaveSalary = table.Column<byte>(type: "tinyint", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "type_leaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_leaves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ConfigKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfigValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: true),
                    IsChangePassword = table.Column<byte>(type: "tinyint", nullable: true),
                    IsActive = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.UniqueConstraint("AK_users_UserCode", x => x.UserCode);
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
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_roles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_UserCode",
                        column: x => x.UserCode,
                        principalTable: "users",
                        principalColumn: "UserCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "superadmin", "SuperAdmin" },
                    { 2, "HR_Manager", "HR Manager" },
                    { 3, "HR", "HR" },
                    { 4, "user", "User" },
                    { 5, "leave_request.approval", "Duyệt đơn nghỉ phép" },
                    { 6, "leave_request.approval_to_hr", "Duyệt đơn nghỉ phép tới HR" },
                    { 7, "leave_request.hr_approval", "HR duyệt đơn nghỉ phép" }
                });

            migrationBuilder.InsertData(
                table: "type_leaves",
                columns: new[] { "Id", "ModifiedAt", "ModifiedBy", "Name", "Note" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), "HR", "Annual", "type_leave.annual" },
                    { 2, new DateTimeOffset(new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), "HR", "Personal", "type_leave.personal" },
                    { 3, new DateTimeOffset(new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), "HR", "Sick", "type_leave.sick" },
                    { 4, new DateTimeOffset(new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), "HR", "Wedding", "type_leave.wedding" },
                    { 5, new DateTimeOffset(new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), "HR", "Accident", "type_leave.accident" },
                    { 6, new DateTimeOffset(new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), "HR", "Other", "type_leave.other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_approval_actions_Id_ApprovalRequestId_ApproverUserCode",
                table: "approval_actions",
                columns: new[] { "Id", "ApprovalRequestId", "ApproverUserCode" });

            migrationBuilder.CreateIndex(
                name: "IX_approval_requests_RequesterUserCode_RequestId_CurrentPositionId_RequestType",
                table: "approval_requests",
                columns: new[] { "RequesterUserCode", "RequestId", "CurrentPositionId", "RequestType" });

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_Id_RequesterUserCode",
                table: "leave_requests",
                columns: new[] { "Id", "RequesterUserCode" });

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Id",
                table: "permissions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_Token_UserCode_ExpiresAt_IsRevoked",
                table: "refresh_tokens",
                columns: new[] { "Token", "UserCode", "ExpiresAt", "IsRevoked" });

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
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_UserCode_RoleId",
                table: "user_roles",
                columns: new[] { "UserCode", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_users_UserCode_Id",
                table: "users",
                columns: new[] { "UserCode", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "approval_actions");

            migrationBuilder.DropTable(
                name: "approval_flows");

            migrationBuilder.DropTable(
                name: "approval_requests");

            migrationBuilder.DropTable(
                name: "leave_requests");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "type_leaves");

            migrationBuilder.DropTable(
                name: "user_configs");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
