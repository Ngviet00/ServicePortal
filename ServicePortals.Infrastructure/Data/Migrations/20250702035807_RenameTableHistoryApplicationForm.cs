using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTableHistoryApplicationForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_approval_actions_application_forms_ApplicationFormId",
                table: "approval_actions");

            migrationBuilder.DropForeignKey(
                name: "FK_approval_actions_application_forms_ApplicationFormId1",
                table: "approval_actions");

            migrationBuilder.DropForeignKey(
                name: "FK_approval_actions_users_UserId",
                table: "approval_actions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_approval_actions",
                table: "approval_actions");

            migrationBuilder.RenameTable(
                name: "approval_actions",
                newName: "history_application_forms");

            migrationBuilder.RenameIndex(
                name: "IX_approval_actions_UserId",
                table: "history_application_forms",
                newName: "IX_history_application_forms_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_approval_actions_ApplicationFormId1",
                table: "history_application_forms",
                newName: "IX_history_application_forms_ApplicationFormId1");

            migrationBuilder.RenameIndex(
                name: "IX_approval_actions_ApplicationFormId_UserApproval",
                table: "history_application_forms",
                newName: "IX_history_application_forms_ApplicationFormId_UserApproval");

            migrationBuilder.AddPrimaryKey(
                name: "PK_history_application_forms",
                table: "history_application_forms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_history_application_forms_application_forms_ApplicationFormId",
                table: "history_application_forms",
                column: "ApplicationFormId",
                principalTable: "application_forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_history_application_forms_application_forms_ApplicationFormId1",
                table: "history_application_forms",
                column: "ApplicationFormId1",
                principalTable: "application_forms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_history_application_forms_users_UserId",
                table: "history_application_forms",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_history_application_forms_application_forms_ApplicationFormId",
                table: "history_application_forms");

            migrationBuilder.DropForeignKey(
                name: "FK_history_application_forms_application_forms_ApplicationFormId1",
                table: "history_application_forms");

            migrationBuilder.DropForeignKey(
                name: "FK_history_application_forms_users_UserId",
                table: "history_application_forms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_history_application_forms",
                table: "history_application_forms");

            migrationBuilder.RenameTable(
                name: "history_application_forms",
                newName: "approval_actions");

            migrationBuilder.RenameIndex(
                name: "IX_history_application_forms_UserId",
                table: "approval_actions",
                newName: "IX_approval_actions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_history_application_forms_ApplicationFormId1",
                table: "approval_actions",
                newName: "IX_approval_actions_ApplicationFormId1");

            migrationBuilder.RenameIndex(
                name: "IX_history_application_forms_ApplicationFormId_UserApproval",
                table: "approval_actions",
                newName: "IX_approval_actions_ApplicationFormId_UserApproval");

            migrationBuilder.AddPrimaryKey(
                name: "PK_approval_actions",
                table: "approval_actions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_approval_actions_application_forms_ApplicationFormId",
                table: "approval_actions",
                column: "ApplicationFormId",
                principalTable: "application_forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_approval_actions_application_forms_ApplicationFormId1",
                table: "approval_actions",
                column: "ApplicationFormId1",
                principalTable: "application_forms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_approval_actions_users_UserId",
                table: "approval_actions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
