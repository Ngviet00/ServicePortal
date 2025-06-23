using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class AddTableMemoNotifyDeptAndColumnApplyAllDept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "memo_notifications",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "memo_notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "ApplyAllDepartment",
                table: "memo_notifications",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyAllDepartment",
                table: "memo_notifications");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "memo_notifications",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "memo_notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
