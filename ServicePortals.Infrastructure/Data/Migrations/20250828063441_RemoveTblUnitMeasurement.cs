using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTblUnitMeasurement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_purchase_details_unit_measurements_UnitMeasurementId",
                table: "purchase_details");

            migrationBuilder.DropTable(
                name: "unit_measurements");

            migrationBuilder.DropIndex(
                name: "IX_purchase_details_UnitMeasurementId",
                table: "purchase_details");

            migrationBuilder.DropColumn(
                name: "UnitMeasurementId",
                table: "purchase_details");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "purchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitMeasurement",
                table: "purchase_details",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "UnitMeasurement",
                table: "purchase_details");

            migrationBuilder.AddColumn<int>(
                name: "UnitMeasurementId",
                table: "purchase_details",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "unit_measurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandFor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unit_measurements", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "unit_measurements",
                columns: new[] { "Id", "Name", "NameE", "StandFor" },
                values: new object[] { 1, "Cái", "Pieces", "pcs" });

            migrationBuilder.CreateIndex(
                name: "IX_purchase_details_UnitMeasurementId",
                table: "purchase_details",
                column: "UnitMeasurementId");

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_details_unit_measurements_UnitMeasurementId",
                table: "purchase_details",
                column: "UnitMeasurementId",
                principalTable: "unit_measurements",
                principalColumn: "Id");
        }
    }
}
