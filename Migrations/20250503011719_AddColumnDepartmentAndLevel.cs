using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnDepartmentAndLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "leave_requests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "level_approval",
                table: "leave_request_steps",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "department_id",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "level_approval",
                table: "leave_request_steps");
        }
    }
}
