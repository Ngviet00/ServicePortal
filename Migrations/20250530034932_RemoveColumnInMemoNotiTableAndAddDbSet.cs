using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnInMemoNotiTableAndAddDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_memo_notifications_FromDate_ToDate_DepartmentIdApply_CreatedByDepartmentId_UserCodeCreated",
                table: "memo_notifications");

            migrationBuilder.DropColumn(
                name: "DepartmentIdApply",
                table: "memo_notifications");

            migrationBuilder.CreateTable(
                name: "memo_notification_departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemoNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memo_notification_departments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_FromDate_ToDate_CreatedByDepartmentId_UserCodeCreated",
                table: "memo_notifications",
                columns: new[] { "FromDate", "ToDate", "CreatedByDepartmentId", "UserCodeCreated" });

            migrationBuilder.CreateIndex(
                name: "IX_memo_notification_departments_MemoNotificationId_DepartmentId",
                table: "memo_notification_departments",
                columns: new[] { "MemoNotificationId", "DepartmentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "memo_notification_departments");

            migrationBuilder.DropIndex(
                name: "IX_memo_notifications_FromDate_ToDate_CreatedByDepartmentId_UserCodeCreated",
                table: "memo_notifications");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentIdApply",
                table: "memo_notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_FromDate_ToDate_DepartmentIdApply_CreatedByDepartmentId_UserCodeCreated",
                table: "memo_notifications",
                columns: new[] { "FromDate", "ToDate", "DepartmentIdApply", "CreatedByDepartmentId", "UserCodeCreated" });
        }
    }
}
