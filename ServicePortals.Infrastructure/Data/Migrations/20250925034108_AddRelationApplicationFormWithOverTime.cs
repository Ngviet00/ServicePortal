using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationApplicationFormWithOverTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_over_times_TypeOverTimeId",
                table: "over_times");

            migrationBuilder.DropColumn(
                name: "TypeOverTimeId",
                table: "over_times");

            migrationBuilder.AddColumn<string>(
                name: "NoteOfHR",
                table: "over_times",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateRegister",
                table: "application_forms",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrgUnitCompanyId",
                table: "application_forms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeOverTimeId",
                table: "application_forms",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_forms_org_units_OrgUnitCompanyId",
                table: "application_forms");

            migrationBuilder.DropForeignKey(
                name: "FK_application_forms_type_over_times_TypeOverTimeId",
                table: "application_forms");

            migrationBuilder.DropIndex(
                name: "IX_application_forms_OrgUnitCompanyId",
                table: "application_forms");

            migrationBuilder.DropIndex(
                name: "IX_application_forms_TypeOverTimeId",
                table: "application_forms");

            migrationBuilder.DropColumn(
                name: "NoteOfHR",
                table: "over_times");

            migrationBuilder.DropColumn(
                name: "DateRegister",
                table: "application_forms");

            migrationBuilder.DropColumn(
                name: "OrgUnitCompanyId",
                table: "application_forms");

            migrationBuilder.DropColumn(
                name: "TypeOverTimeId",
                table: "application_forms");

            migrationBuilder.AddColumn<int>(
                name: "TypeOverTimeId",
                table: "over_times",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_over_times_TypeOverTimeId",
                table: "over_times",
                column: "TypeOverTimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_over_times_type_over_times_TypeOverTimeId",
                table: "over_times",
                column: "TypeOverTimeId",
                principalTable: "type_over_times",
                principalColumn: "Id");
        }
    }
}
