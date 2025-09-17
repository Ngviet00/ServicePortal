using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnDepartmentIdTblApplicationForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "application_forms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_DepartmentId",
                table: "application_forms",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_forms_org_units_DepartmentId",
                table: "application_forms");

            migrationBuilder.DropIndex(
                name: "IX_application_forms_DepartmentId",
                table: "application_forms");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "application_forms");
        }
    }
}
