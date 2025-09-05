using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationshipApplicationFormAndOrgUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_application_forms_DepartmentId",
                table: "application_forms");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_DepartmentId",
                table: "application_forms",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_application_forms_DepartmentId",
                table: "application_forms");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_DepartmentId",
                table: "application_forms",
                column: "DepartmentId",
                unique: true,
                filter: "[DepartmentId] IS NOT NULL");
        }
    }
}
