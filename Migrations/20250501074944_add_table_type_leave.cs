using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class add_table_type_leave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "type_leaves",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    modified_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    modified_at = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_leaves", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "type_leaves",
                columns: new[] { "id", "modified_at", "modified_by", "name", "note" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Annual", "type_leave.annual" },
                    { 2, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Personal", "type_leave.personal" },
                    { 3, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Sick", "type_leave.sick" },
                    { 4, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Wedding", "type_leave.wedding" },
                    { 5, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Accident", "type_leave.accident" },
                    { 6, new DateTime(2025, 5, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "HR", "Other", "type_leave.other" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "type_leaves");
        }
    }
}
