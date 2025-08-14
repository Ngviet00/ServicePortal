using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnNameEnglishRequestType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameE",
                table: "request_types",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "request_types",
                keyColumn: "Id",
                keyValue: 1,
                column: "NameE",
                value: null);

            migrationBuilder.UpdateData(
                table: "request_types",
                keyColumn: "Id",
                keyValue: 2,
                column: "NameE",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameE",
                table: "request_types");
        }
    }
}
