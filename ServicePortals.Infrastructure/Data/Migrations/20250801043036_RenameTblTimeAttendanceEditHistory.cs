using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTblTimeAttendanceEditHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeAttendanceEditHistories",
                table: "TimeAttendanceEditHistories");

            migrationBuilder.RenameTable(
                name: "TimeAttendanceEditHistories",
                newName: "time_attendance_edit_histories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_time_attendance_edit_histories",
                table: "time_attendance_edit_histories",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_time_attendance_edit_histories",
                table: "time_attendance_edit_histories");

            migrationBuilder.RenameTable(
                name: "time_attendance_edit_histories",
                newName: "TimeAttendanceEditHistories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeAttendanceEditHistories",
                table: "TimeAttendanceEditHistories",
                column: "Id");
        }
    }
}
