using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnCode_BgColor_TextColor_tblTypeLeaves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BgColor",
                table: "type_leaves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "type_leaves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextColor",
                table: "type_leaves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BgColor", "Code", "TextColor" },
                values: new object[] { "#E1CD00", "AL", "#FFFFFF" });

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BgColor", "Code", "TextColor" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BgColor", "Code", "TextColor" },
                values: new object[] { "#E800FF", "MC", "#FFFFFF" });

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BgColor", "Code", "TextColor" },
                values: new object[] { "#9685FF", "ML", "#FFFFFF"});

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BgColor", "Code", "TextColor" },
                values: new object[] { "#FF6700", "ACC", "#FFFFFF" });

            migrationBuilder.UpdateData(
                table: "type_leaves",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BgColor", "Code", "TextColor" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BgColor",
                table: "type_leaves");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "type_leaves");

            migrationBuilder.DropColumn(
                name: "TextColor",
                table: "type_leaves");
        }
    }
}
