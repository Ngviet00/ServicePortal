using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTblPositionToOrgPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_forms_positions_PositionId",
                table: "application_forms");

            migrationBuilder.DropTable(
                name: "positions");

            migrationBuilder.CreateTable(
                name: "org_positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgUnitId = table.Column<int>(type: "int", nullable: true),
                    ParentPositionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_org_positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_org_positions_org_units_OrgUnitId",
                        column: x => x.OrgUnitId,
                        principalTable: "org_units",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "org_positions",
                columns: new[] { "Id", "Name", "OrgUnitId", "ParentPositionId", "PositionCode" },
                values: new object[,]
                {
                    { 1, "General Director", 6, null, "GD" },
                    { 2, "AM General Director", 6, 1, "AM_GD" },
                    { 3, "BD General Manager", 6, 1, "BDGM" },
                    { 4, "Finance General Manage", 6, 1, "FGM" },
                    { 5, "Operations General Manager", 6, 1, "OGM" },
                    { 6, "Operations Manager", 6, 1, "OM" },
                    { 7, "Manager MIS/IT", 8, null, "MIS-MGR" },
                    { 8, "Staff IT", 8, 7, "MIS-Staff" },
                    { 9, "Manager Commercial", 10, null, "COM-MGR" },
                    { 10, "AM Commercial", 10, 9, "COM-AM" },
                    { 11, "Staff Commercial", 10, 10, "COM-Staff" },
                    { 12, "Manager HR", 9, null, "HR-MGR" },
                    { 13, "AM HR", 9, 12, "HR-AM" },
                    { 14, "Staff HR", 9, 13, "HR-Staff" },
                    { 15, "Manager Production", 7, null, "PRD-MGR" },
                    { 16, "Supervisor A_AGH", 18, 15, "PRD-S-AGH" },
                    { 17, "Supervisor B_BCDEF", 19, 15, "PRD-S-BBCDEF" },
                    { 18, "Supervisor Shift A", 14, 15, "PRD-S-SA" },
                    { 19, "Supervisor Shift B", 17, 15, "PRD-S-SB" },
                    { 20, "12A_A Leader", 14, 18, "PRD-12AA-L" },
                    { 21, "12A_A Operator", 14, 18, "PRD-12AA-OP" },
                    { 22, "12B_A Leader", 16, 19, "PRD-12BA-L" },
                    { 23, "12B_A Operator", 16, 19, "PRD-12BA-OP" },
                    { 24, "Technician A_AGH", 18, 16, "PRD-T-AAH" },
                    { 25, "Technician B_BCDEF", 19, 17, "PRD-T-BCDEF" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_org_positions_OrgUnitId",
                table: "org_positions",
                column: "OrgUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_application_forms_org_positions_PositionId",
                table: "application_forms",
                column: "PositionId",
                principalTable: "org_positions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_forms_org_positions_PositionId",
                table: "application_forms");

            migrationBuilder.DropTable(
                name: "org_positions");

            migrationBuilder.CreateTable(
                name: "positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrgUnitId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentPositionId = table.Column<int>(type: "int", nullable: true),
                    PositionCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_positions_org_units_OrgUnitId",
                        column: x => x.OrgUnitId,
                        principalTable: "org_units",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "positions",
                columns: new[] { "Id", "Name", "OrgUnitId", "ParentPositionId", "PositionCode" },
                values: new object[,]
                {
                    { 1, "General Director", 6, null, "GD" },
                    { 2, "AM General Director", 6, 1, "AM_GD" },
                    { 3, "BD General Manager", 6, 1, "BDGM" },
                    { 4, "Finance General Manage", 6, 1, "FGM" },
                    { 5, "Operations General Manager", 6, 1, "OGM" },
                    { 6, "Operations Manager", 6, 1, "OM" },
                    { 7, "Manager MIS/IT", 8, null, "MIS-MGR" },
                    { 8, "Staff IT", 8, 7, "MIS-Staff" },
                    { 9, "Manager Commercial", 10, null, "COM-MGR" },
                    { 10, "AM Commercial", 10, 9, "COM-AM" },
                    { 11, "Staff Commercial", 10, 10, "COM-Staff" },
                    { 12, "Manager HR", 9, null, "HR-MGR" },
                    { 13, "AM HR", 9, 12, "HR-AM" },
                    { 14, "Staff HR", 9, 13, "HR-Staff" },
                    { 15, "Manager Production", 7, null, "PRD-MGR" },
                    { 16, "Supervisor A_AGH", 18, 15, "PRD-S-AGH" },
                    { 17, "Supervisor B_BCDEF", 19, 15, "PRD-S-BBCDEF" },
                    { 18, "Supervisor Shift A", 14, 15, "PRD-S-SA" },
                    { 19, "Supervisor Shift B", 17, 15, "PRD-S-SB" },
                    { 20, "12A_A Leader", 14, 18, "PRD-12AA-L" },
                    { 21, "12A_A Operator", 14, 18, "PRD-12AA-OP" },
                    { 22, "12B_A Leader", 16, 19, "PRD-12BA-L" },
                    { 23, "12B_A Operator", 16, 19, "PRD-12BA-OP" },
                    { 24, "Technician A_AGH", 18, 16, "PRD-T-AAH" },
                    { 25, "Technician B_BCDEF", 19, 17, "PRD-T-BCDEF" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_positions_OrgUnitId",
                table: "positions",
                column: "OrgUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_application_forms_positions_PositionId",
                table: "application_forms",
                column: "PositionId",
                principalTable: "positions",
                principalColumn: "Id");
        }
    }
}
