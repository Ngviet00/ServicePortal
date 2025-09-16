using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnApplcationFormToApplicationFormItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_memo_notifications_application_forms_ApplicationFormId",
                table: "memo_notifications");

            migrationBuilder.RenameColumn(
                name: "ApplicationFormId",
                table: "memo_notifications",
                newName: "ApplicationFormItemId");

            migrationBuilder.RenameIndex(
                name: "IX_memo_notifications_ApplicationFormId",
                table: "memo_notifications",
                newName: "IX_memo_notifications_ApplicationFormItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_memo_notifications_application_form_items_ApplicationFormItemId",
                table: "memo_notifications");

            migrationBuilder.RenameColumn(
                name: "ApplicationFormItemId",
                table: "memo_notifications",
                newName: "ApplicationFormId");

            migrationBuilder.RenameIndex(
                name: "IX_memo_notifications_ApplicationFormItemId",
                table: "memo_notifications",
                newName: "IX_memo_notifications_ApplicationFormId");
        }
    }
}
