using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnNameEnglishTypeLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameE",
                table: "type_leaves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 1,
                column: "NameE",
                value: null);

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 2,
                column: "NameE",
                value: null);

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 3,
                column: "NameE",
                value: null);

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 4,
                column: "NameE",
                value: null);

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 5,
                column: "NameE",
                value: null);

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 6,
                column: "NameE",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameE",
                table: "type_leaves");
        }
    }
}
