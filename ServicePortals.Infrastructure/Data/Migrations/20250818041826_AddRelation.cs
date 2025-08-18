using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_memo_notifications_org_units_OrgUnitId",
                table: "memo_notifications");

            migrationBuilder.DropIndex(
                name: "IX_memo_notifications_OrgUnitId",
                table: "memo_notifications");

            migrationBuilder.DropColumn(
                name: "OrgUnitId",
                table: "memo_notifications");

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_DepartmentId",
                table: "memo_notifications",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_memo_notifications_org_units_DepartmentId",
                table: "memo_notifications");

            migrationBuilder.DropIndex(
                name: "IX_memo_notifications_DepartmentId",
                table: "memo_notifications");

            migrationBuilder.AddColumn<int>(
                name: "OrgUnitId",
                table: "memo_notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_OrgUnitId",
                table: "memo_notifications",
                column: "OrgUnitId");
        }
    }
}
