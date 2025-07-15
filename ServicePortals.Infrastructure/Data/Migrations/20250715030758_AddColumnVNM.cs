using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnVNM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NameE",
                table: "type_leaves",
                newName: "NameV");

            migrationBuilder.DropColumn(
                name: "English",
                table: "time_leaves");

            migrationBuilder.AddColumn<string>(
                name: "English",
                table: "time_leaves",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.InsertData(
                table: "request_statuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "PENDING" },
                    { 2, "IN_PROCESS" },
                    { 3, "COMPLETE" },
                    { 4, "WAIT_HR" },
                    { 5, "REJECT" }
                });

            migrationBuilder.InsertData(
                table: "request_types",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Nghỉ Phép" },
                    { 2, "Chấm Công" }
                });

            migrationBuilder.InsertData(
                table: "time_leaves",
                columns: new[] { "Id", "Description", "English" },
                values: new object[,]
                {
                    { 1, "Cả Ngày", "All Day" },
                    { 2, "Buổi Sáng", "Morning" },
                    { 3, "Buổi Chiều", "Afternoon" }
                });

            migrationBuilder.InsertData(
                table: "type_leaves",
                columns: new[] { "Id", "Name", "Note", "NameV" },
                values: new object[,]
                {
                    { 1, "Annual", null, "Nghỉ Phép Năm" },
                    { 2, "Personal", null, "Nghỉ Việc Cá Nhân" },
                    { 3, "Sick", null, "Nghỉ Ốm" },
                    { 4, "Wedding", null, "Nghỉ Cưới" },
                    { 5, "Accident", null, "Nghỉ TNLĐ" },
                    { 6, "Other", null, "Khác" },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "English",
                table: "time_leaves");

        }
    }
}
