using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameNationalityTblUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NationNality",
                table: "users",
                newName: "Nationality");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nationality",
                table: "users",
                newName: "NationNality");
        }
    }
}
