using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTblAssignedTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_it_forms_ApplicationFormId",
                table: "it_forms");

            migrationBuilder.CreateTable(
                name: "assigned_tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assigned_tasks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_ApplicationFormId",
                table: "it_forms",
                column: "ApplicationFormId",
                unique: true,
                filter: "[ApplicationFormId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_assigned_tasks_ApplicationFormId",
                table: "assigned_tasks",
                column: "ApplicationFormId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assigned_tasks");

            migrationBuilder.DropIndex(
                name: "IX_it_forms_ApplicationFormId",
                table: "it_forms");

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_ApplicationFormId",
                table: "it_forms",
                column: "ApplicationFormId");
        }
    }
}
