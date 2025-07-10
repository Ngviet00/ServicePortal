using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RecreateRelationUserPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_permissions_UserId",
                table: "user_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_permissions_users_UserId",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "user_permissions");

            migrationBuilder.AddColumn<string>(
                name: "UserCode1",
                table: "user_permissions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_UserCode1",
                table: "user_permissions",
                column: "UserCode1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_permissions_users_UserCode1",
                table: "user_permissions");

            migrationBuilder.DropIndex(
                name: "IX_user_permissions_UserCode1",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "UserCode1",
                table: "user_permissions");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "user_permissions",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
