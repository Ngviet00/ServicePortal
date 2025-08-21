using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnUserNameInTblITForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherCategory",
                table: "it_forms",
                newName: "UserNameRequestor");

            migrationBuilder.AddColumn<string>(
                name: "UserNameCreated",
                table: "it_forms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserNameCreated",
                table: "it_forms");

            migrationBuilder.RenameColumn(
                name: "UserNameRequestor",
                table: "it_forms",
                newName: "OtherCategory");
        }
    }
}
