using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropTableMemoNoti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_memo_notification_departments_memo_notifications_MemoNotificationId",
                table: "memo_notification_departments");

            migrationBuilder.DropTable(name: "memo_notification_departments");

            migrationBuilder.DropTable(name: "memo_notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
