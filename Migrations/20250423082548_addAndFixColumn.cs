using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class addAndFixColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "users",
                newName: "parent_department_id");

            migrationBuilder.AddColumn<int>(
                name: "child_department_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "management_position_id",
                table: "users",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "child_department_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "management_position_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "parent_department_id",
                table: "users",
                newName: "department_id");
        }
    }
}
