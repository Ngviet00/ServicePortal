using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class initDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "approval_leave_request_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    leave_request_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    user_code_approver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    order = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_leave_request_steps", x => x.id);
                });

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
                name: "leave_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name_register = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    position_id = table.Column<int>(type: "int", nullable: true),
                    deparment_id = table.Column<int>(type: "int", nullable: true),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    from_hour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    from_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    to_hour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    to_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    state = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    display_hr = table.Column<bool>(type: "bit", nullable: true),
                    type_leave = table.Column<int>(type: "int", nullable: true),
                    reason_type_leave_other = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    time_leave = table.Column<int>(type: "int", nullable: true),
                    have_salary = table.Column<bool>(type: "bit", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    meta_data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_requests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "position_departments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    position_id = table.Column<int>(type: "int", nullable: true),
                    position_department_level = table.Column<int>(type: "int", nullable: true),
                    custom_title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_position_departments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    position_level = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_positions", x => x.id);
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
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_assignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    position_department_id = table.Column<int>(type: "int", nullable: true),
                    is_head_of_deparment = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_assignments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    date_join_company = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "departments",
                columns: new[] { "id", "name", "note", "parent_id" },
                values: new object[] { 1, "IT/MIS", "IT", null });

            migrationBuilder.InsertData(
                table: "position_departments",
                columns: new[] { "id", "custom_title", "department_id", "position_department_level", "position_id" },
                values: new object[] { 1, "Manager IT/MIS", 1, 1, 4 });

            migrationBuilder.InsertData(
                table: "positions",
                columns: new[] { "id", "name", "position_level" },
                values: new object[,]
                {
                    { -1, "SuperAdmin", 0 },
                    { 1, "General Director", 1 },
                    { 2, "Assistant General Director", 2 },
                    { 3, "General Manager", 3 },
                    { 4, "Manager", 4 },
                    { 5, "Assistant Manager", 5 },
                    { 6, "Supervisor", 6 },
                    { 7, "Chief Accountant", 6 },
                    { 8, "Staff", 7 }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "SuperAdmin" },
                    { 2, "IT" },
                    { 3, "HR" },
                    { 4, "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_departments_id_parent_id",
                table: "departments",
                columns: new[] { "id", "parent_id" });

            migrationBuilder.CreateIndex(
                name: "IX_position_departments_department_id_position_id_position_department_level",
                table: "position_departments",
                columns: new[] { "department_id", "position_id", "position_department_level" });

            migrationBuilder.CreateIndex(
                name: "IX_positions_id",
                table: "positions",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token_user_code_expires_at_is_revoked",
                table: "refresh_tokens",
                columns: new[] { "token", "user_code", "expires_at", "is_revoked" });

            migrationBuilder.CreateIndex(
                name: "IX_roles_id",
                table: "roles",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_user_assignments_user_code_position_department_id",
                table: "user_assignments",
                columns: new[] { "user_code", "position_department_id" });

            migrationBuilder.CreateIndex(
                name: "IX_users_code_email_id",
                table: "users",
                columns: new[] { "code", "email", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "approval_leave_request_steps");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "leave_requests");

            migrationBuilder.DropTable(
                name: "position_departments");

            migrationBuilder.DropTable(
                name: "positions");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "user_assignments");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
