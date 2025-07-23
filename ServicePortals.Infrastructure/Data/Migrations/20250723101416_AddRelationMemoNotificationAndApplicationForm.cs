using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationMemoNotificationAndApplicationForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationFormId",
                table: "memo_notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_memo_notifications_ApplicationFormId",
                table: "memo_notifications",
                column: "ApplicationFormId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_memo_notifications_ApplicationFormId",
                table: "memo_notifications");

            migrationBuilder.DropColumn(
                name: "ApplicationFormId",
                table: "memo_notifications");
        }
    }
}
