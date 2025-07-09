using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationHistoryApplicationForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_history_application_forms_application_forms_ApplicationFormId1",
                table: "history_application_forms");

            migrationBuilder.DropIndex(
                name: "IX_history_application_forms_ApplicationFormId1",
                table: "history_application_forms");

            migrationBuilder.DropColumn(
                name: "ApplicationFormId1",
                table: "history_application_forms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationFormId1",
                table: "history_application_forms",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_history_application_forms_ApplicationFormId1",
                table: "history_application_forms",
                column: "ApplicationFormId1");

            migrationBuilder.AddForeignKey(
                name: "FK_history_application_forms_application_forms_ApplicationFormId1",
                table: "history_application_forms",
                column: "ApplicationFormId1",
                principalTable: "application_forms",
                principalColumn: "Id");
        }
    }
}
