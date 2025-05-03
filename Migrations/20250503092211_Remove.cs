using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class Remove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "leave_requests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_UserId",
                table: "leave_requests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_leave_requests_users_UserId",
                table: "leave_requests",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leave_requests_users_UserId",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "IX_leave_requests_UserId",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "leave_requests");
        }
    }
}
