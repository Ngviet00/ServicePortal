using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class AllTableLeaveRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "leave_request_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    leave_request_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    user_code_approver = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    status_step = table.Column<byte>(type: "tinyint", nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    approved_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_request_steps", x => x.id);
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
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_requests", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_leave_request_steps_id_leave_request_id_user_code_approver_status_step",
                table: "leave_request_steps",
                columns: new[] { "id", "leave_request_id", "user_code_approver", "status_step" });

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_id_user_code",
                table: "leave_requests",
                columns: new[] { "id", "user_code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "leave_request_steps");

            migrationBuilder.DropTable(
                name: "leave_requests");
        }
    }
}
