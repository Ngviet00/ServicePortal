using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrgUnitContextAndAddColumnStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ToOrgUnitContext",
                table: "work_flow_steps",
                newName: "OrgUnitContext");

            migrationBuilder.AddColumn<int>(
                name: "Step",
                table: "work_flow_steps",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrgUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeptId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    ParentOrgUnitId = table.Column<int>(type: "int", nullable: true),
                    ParentJobTitleId = table.Column<int>(type: "int", nullable: true),
                    ManagerUserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeputyUserCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnits", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrgUnits");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "work_flow_steps");

            migrationBuilder.RenameColumn(
                name: "OrgUnitContext",
                table: "work_flow_steps",
                newName: "ToOrgUnitContext");
        }
    }
}
