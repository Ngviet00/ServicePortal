using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnPositionToOrgPositionTblApplicationForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_forms_org_positions_PositionId",
                table: "application_forms");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "application_forms",
                newName: "OrgPositionId");

            migrationBuilder.RenameIndex(
                name: "IX_application_forms_PositionId",
                table: "application_forms",
                newName: "IX_application_forms_OrgPositionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_forms_org_positions_OrgPositionId",
                table: "application_forms");

            migrationBuilder.RenameColumn(
                name: "OrgPositionId",
                table: "application_forms",
                newName: "PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_application_forms_OrgPositionId",
                table: "application_forms",
                newName: "IX_application_forms_PositionId");
        }
    }
}
