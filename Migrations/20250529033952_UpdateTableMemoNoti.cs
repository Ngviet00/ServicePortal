using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableMemoNoti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_memo_notifications_FromDate_ToDate_DepartmentId_UserCodeCreated",
                table: "memo_notifications");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "memo_notifications",
                newName: "DepartmentIdApply");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByDepartmentId",
                table: "memo_notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "memo_notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_FromDate_ToDate_DepartmentIdApply_CreatedByDepartmentId_UserCodeCreated",
                table: "memo_notifications",
                columns: new[] { "FromDate", "ToDate", "DepartmentIdApply", "CreatedByDepartmentId", "UserCodeCreated" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_memo_notifications_FromDate_ToDate_DepartmentIdApply_CreatedByDepartmentId_UserCodeCreated",
                table: "memo_notifications");

            migrationBuilder.DropColumn(
                name: "CreatedByDepartmentId",
                table: "memo_notifications");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "memo_notifications");

            migrationBuilder.RenameColumn(
                name: "DepartmentIdApply",
                table: "memo_notifications",
                newName: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_FromDate_ToDate_DepartmentId_UserCodeCreated",
                table: "memo_notifications",
                columns: new[] { "FromDate", "ToDate", "DepartmentId", "UserCodeCreated" });
        }
    }
}
