using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTblMissTimeKeeping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "miss_timekeepings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormItemId = table.Column<long>(type: "bigint", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateRegister = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Shift = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AdditionalIn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AdditionalOut = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FacialRecognitionIn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FacialRecognitionOut = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_miss_timekeepings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_miss_timekeepings_ApplicationFormItemId",
                table: "miss_timekeepings",
                column: "ApplicationFormItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "miss_timekeepings");
        }
    }
}
