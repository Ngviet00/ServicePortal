using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnNameLRQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "leave_requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "leave_requests");
        }
    }
}
