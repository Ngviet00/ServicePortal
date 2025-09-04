using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeColumnQtyIntToDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "purchase_details",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "purchase_details",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
