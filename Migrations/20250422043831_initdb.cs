using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
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
                name: "positions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    level = table.Column<int>(type: "int", nullable: true),
                    is_global = table.Column<bool>(type: "bit", nullable: true)
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
                name: "teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.id);
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
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sex = table.Column<byte>(type: "tinyint", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    position_id = table.Column<int>(type: "int", nullable: true),
                    team_id = table.Column<int>(type: "int", nullable: true),
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
                table: "positions",
                columns: new[] { "id", "department_id", "is_global", "level", "name", "title" },
                values: new object[,]
                {
                    { 1, 0, true, -2, "General Director", "General Director" },
                    { 2, 0, true, -1, "Assistant General Director", "Assistant General Director" },
                    { 3, 1, true, 0, "Superadmin", "Superadmin" },
                    { 4, 1, false, 1, "Manager", "Manager IT/MIS" }
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
                name: "IX_users_code_email_id",
                table: "users",
                columns: new[] { "code", "email", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "positions");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
