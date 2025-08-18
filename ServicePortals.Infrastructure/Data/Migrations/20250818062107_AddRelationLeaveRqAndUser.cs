using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationLeaveRqAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserCodeRequestor",
                table: "leave_requests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_UserCodeRequestor",
                table: "leave_requests",
                column: "UserCodeRequestor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leave_requests_users_UserCodeRequestor",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "IX_leave_requests_UserCodeRequestor",
                table: "leave_requests");

            migrationBuilder.AlterColumn<string>(
                name: "UserCodeRequestor",
                table: "leave_requests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
