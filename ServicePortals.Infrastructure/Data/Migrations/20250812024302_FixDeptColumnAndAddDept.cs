using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixDeptColumnAndAddDept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "leave_requests");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "memo_notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "leave_requests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_DepartmentId",
                table: "memo_notifications",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_DepartmentId",
                table: "leave_requests",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_memo_notifications_DepartmentId",
                table: "memo_notifications");

            migrationBuilder.DropIndex(
                name: "IX_leave_requests_DepartmentId",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "memo_notifications");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "leave_requests");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "leave_requests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
