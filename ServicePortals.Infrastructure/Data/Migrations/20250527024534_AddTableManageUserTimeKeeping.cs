using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class AddTableManageUserTimeKeeping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "manage_user_time_keeping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCodeManage = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_manage_user_time_keeping", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_manage_user_time_keeping_UserCodeManage_UserCode",
                table: "manage_user_time_keeping",
                columns: new[] { "UserCodeManage", "UserCode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "manage_user_time_keeping");
        }
    }
}
