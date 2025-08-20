using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNameDescriptionToCodeTblITCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "it_categories",
                newName: "Code");

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Code",
                value: "SERVER");

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Code",
                value: "NETWORK");

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Code",
                value: "EMAIL");

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Code",
                value: "SOFTWARE");

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Code",
                value: "ERP");

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "Code",
                value: "OTHER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "it_categories",
                newName: "Description");

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "it_categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "Description",
                value: null);
        }
    }
}
