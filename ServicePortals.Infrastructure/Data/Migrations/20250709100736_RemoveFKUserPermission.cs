using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFKUserPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_permissions_users_UserCode",
                table: "user_permissions");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "user_permissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_UserId",
                table: "user_permissions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_permissions_users_UserId",
                table: "user_permissions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_permissions_users_UserId",
                table: "user_permissions");

            migrationBuilder.DropIndex(
                name: "IX_user_permissions_UserId",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "user_permissions");

            migrationBuilder.AddForeignKey(
                name: "FK_user_permissions_users_UserCode",
                table: "user_permissions",
                column: "UserCode",
                principalTable: "users",
                principalColumn: "UserCode",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
