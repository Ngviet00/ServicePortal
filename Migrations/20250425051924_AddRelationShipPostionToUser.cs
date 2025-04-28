using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationShipPostionToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_users_position_id",
                table: "users",
                column: "position_id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_positions_position_id",
                table: "users",
                column: "position_id",
                principalTable: "positions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_positions_position_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_position_id",
                table: "users");
        }
    }
}
