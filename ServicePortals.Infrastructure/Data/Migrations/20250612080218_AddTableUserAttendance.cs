using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class AddTableUserAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "manage_user_time_keeping");

            migrationBuilder.CreateTable(
                name: "user_manage_attendance_users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCodeManage = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_manage_attendance_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_manage_attendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_manage_attendances", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_manage_attendance_users_UserCodeManage_UserCode",
                table: "user_manage_attendance_users",
                columns: new[] { "UserCodeManage", "UserCode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_manage_attendance_users");

            migrationBuilder.DropTable(
                name: "user_manage_attendances");

            migrationBuilder.CreateTable(
                name: "manage_user_time_keeping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserCodeManage = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
    }
}
