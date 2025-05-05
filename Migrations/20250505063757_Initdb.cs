using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class Initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    parent_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    token = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    user_code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    is_revoked = table.Column<bool>(type: "bit", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "type_leaves",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    modified_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    modified_at = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_leaves", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    date_join_company = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sex = table.Column<byte>(type: "tinyint", nullable: true),
                    position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    level_parent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.UniqueConstraint("AK_users_code", x => x.code);
                    table.ForeignKey(
                        name: "FK_users_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    permission_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "leave_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_code_register = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name_register = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deparment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    from_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    to_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    time_leave = table.Column<byte>(type: "tinyint", nullable: true),
                    type_leave = table.Column<byte>(type: "tinyint", nullable: true),
                    status = table.Column<byte>(type: "tinyint", nullable: true),
                    have_salary = table.Column<bool>(type: "bit", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_leave_requests_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    permission_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_permissions_users_user_code",
                        column: x => x.user_code,
                        principalTable: "users",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_roles_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_code",
                        column: x => x.user_code,
                        principalTable: "users",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "leave_request_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    leave_request_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    user_code_approver = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    status_step = table.Column<byte>(type: "tinyint", nullable: true),
                    level_approval = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    approved_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_request_steps", x => x.id);
                    table.ForeignKey(
                        name: "FK_leave_request_steps_leave_requests_leave_request_id",
                        column: x => x.leave_request_id,
                        principalTable: "leave_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "departments",
                columns: new[] { "id", "name", "note", "parent_id" },
                values: new object[,]
                {
                    { 1, "IT/MIS", "department.IT", null },
                    { 2, "HR", "department.HR", null },
                    { 3, "Sản xuất", "department.production", null }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { 1, null, "SuperAdmin" },
                    { 3, null, "HR" },
                    { 4, null, "User" }
                });

            migrationBuilder.InsertData(
                table: "type_leaves",
                columns: new[] { "id", "modified_at", "modified_by", "name", "note" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Annual", "type_leave.annual" },
                    { 2, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Personal", "type_leave.personal" },
                    { 3, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Sick", "type_leave.sick" },
                    { 4, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Wedding", "type_leave.wedding" },
                    { 5, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Accident", "type_leave.accident" },
                    { 6, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Other", "type_leave.other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_departments_id_parent_id",
                table: "departments",
                columns: new[] { "id", "parent_id" });

            migrationBuilder.CreateIndex(
                name: "IX_leave_request_steps_id_leave_request_id_user_code_approver_status_step",
                table: "leave_request_steps",
                columns: new[] { "id", "leave_request_id", "user_code_approver", "status_step" });

            migrationBuilder.CreateIndex(
                name: "IX_leave_request_steps_leave_request_id",
                table: "leave_request_steps",
                column: "leave_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_id_user_code",
                table: "leave_requests",
                columns: new[] { "id", "user_code" });

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_UserId",
                table: "leave_requests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_id",
                table: "permissions",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token_user_code_expires_at_is_revoked",
                table: "refresh_tokens",
                columns: new[] { "token", "user_code", "expires_at", "is_revoked" });

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_role_id_permission_id",
                table: "role_permissions",
                columns: new[] { "role_id", "permission_id" });

            migrationBuilder.CreateIndex(
                name: "IX_roles_id",
                table: "roles",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_permission_id",
                table: "user_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_user_code_permission_id",
                table: "user_permissions",
                columns: new[] { "user_code", "permission_id" });

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_department_id",
                table: "user_roles",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_user_code_role_id_department_id",
                table: "user_roles",
                columns: new[] { "user_code", "role_id", "department_id" });

            migrationBuilder.CreateIndex(
                name: "IX_users_code_email_id",
                table: "users",
                columns: new[] { "code", "email", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_users_department_id",
                table: "users",
                column: "department_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "leave_request_steps");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "type_leaves");

            migrationBuilder.DropTable(
                name: "user_permissions");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "leave_requests");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "departments");
        }
    }
}
