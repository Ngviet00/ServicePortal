using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableUnitAndColumnAssignUserCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "work_flow_steps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNeedHighLevel",
                table: "work_flow_steps",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignUserCode",
                table: "application_forms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "work_flow_steps");

            migrationBuilder.DropColumn(
                name: "IsNeedHighLevel",
                table: "work_flow_steps");

            migrationBuilder.DropColumn(
                name: "AssignUserCode",
                table: "application_forms");
        }
    }
}
