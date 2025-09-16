using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnApplcationFormToApplicationFormItemTblITFormAndPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_it_forms_application_forms_ApplicationFormId",
                table: "it_forms");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_application_forms_ApplicationFormId",
                table: "purchases");

            migrationBuilder.RenameColumn(
                name: "ApplicationFormId",
                table: "purchases",
                newName: "ApplicationFormItemId");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_ApplicationFormId",
                table: "purchases",
                newName: "IX_purchases_ApplicationFormItemId");

            migrationBuilder.RenameColumn(
                name: "ApplicationFormId",
                table: "it_forms",
                newName: "ApplicationFormItemId");

            migrationBuilder.RenameIndex(
                name: "IX_it_forms_ApplicationFormId",
                table: "it_forms",
                newName: "IX_it_forms_ApplicationFormItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_it_forms_application_form_items_ApplicationFormItemId",
                table: "it_forms");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_application_form_items_ApplicationFormItemId",
                table: "purchases");

            migrationBuilder.RenameColumn(
                name: "ApplicationFormItemId",
                table: "purchases",
                newName: "ApplicationFormId");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_ApplicationFormItemId",
                table: "purchases",
                newName: "IX_purchases_ApplicationFormId");

            migrationBuilder.RenameColumn(
                name: "ApplicationFormItemId",
                table: "it_forms",
                newName: "ApplicationFormId");

            migrationBuilder.RenameIndex(
                name: "IX_it_forms_ApplicationFormItemId",
                table: "it_forms",
                newName: "IX_it_forms_ApplicationFormId");
        }
    }
}
