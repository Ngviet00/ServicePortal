using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationUserAndUserConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_configs_users_UserId",
                table: "user_configs");

            migrationBuilder.DropIndex(
                name: "IX_user_configs_UserId",
                table: "user_configs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "user_configs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "user_configs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_configs_UserId",
                table: "user_configs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_configs_users_UserId",
                table: "user_configs",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
