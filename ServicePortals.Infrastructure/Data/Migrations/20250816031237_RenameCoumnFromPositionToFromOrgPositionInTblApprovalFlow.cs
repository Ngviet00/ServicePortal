using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameCoumnFromPositionToFromOrgPositionInTblApprovalFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_approval_flows_FromPositionId",
                table: "approval_flows");

            migrationBuilder.RenameColumn(
                name: "ToPositionId",
                table: "approval_flows",
                newName: "ToOrgPositionId");

            migrationBuilder.RenameColumn(
                name: "FromPositionId",
                table: "approval_flows",
                newName: "FromOrgPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_approval_flows_FromOrgPositionId_ToOrgPositionId",
                table: "approval_flows",
                columns: new[] { "FromOrgPositionId", "ToOrgPositionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_approval_flows_FromOrgPositionId_ToOrgPositionId",
                table: "approval_flows");

            migrationBuilder.RenameColumn(
                name: "ToOrgPositionId",
                table: "approval_flows",
                newName: "ToPositionId");

            migrationBuilder.RenameColumn(
                name: "FromOrgPositionId",
                table: "approval_flows",
                newName: "FromPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_approval_flows_FromPositionId",
                table: "approval_flows",
                column: "FromPositionId");
        }
    }
}
