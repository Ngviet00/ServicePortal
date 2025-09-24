using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTblOverTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "type_over_times",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_over_times", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "over_times",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormItemId = table.Column<long>(type: "bigint", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FromHour = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ToHour = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NumberHour = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOverTimeId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_over_times", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "type_over_times",
                columns: new[] { "Id", "Name", "NameE" },
                values: new object[,]
                {
                    { 1, "Từ thứ 2 đến thứ 7", "Form monday to saturday" },
                    { 2, "Chủ nhật", "Sunday" },
                    { 3, "Ngày lễ", "Holiday" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_over_times_ApplicationFormItemId",
                table: "over_times",
                column: "ApplicationFormItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "over_times");

            migrationBuilder.DropTable(
                name: "type_over_times");
        }
    }
}
