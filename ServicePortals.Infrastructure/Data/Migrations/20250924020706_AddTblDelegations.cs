using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTblDelegations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPermanent",
                table: "delegations");

            migrationBuilder.RenameColumn(
                name: "ToUserCode",
                table: "delegations",
                newName: "UserCodeDelegation");

            migrationBuilder.RenameColumn(
                name: "ToDate",
                table: "delegations",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "delegations",
                newName: "FromOrgPositionId");

            migrationBuilder.RenameColumn(
                name: "FromUserCode",
                table: "delegations",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "FromDate",
                table: "delegations",
                newName: "EndDate");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "delegations",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "delegations",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "delegations");

            migrationBuilder.RenameColumn(
                name: "UserCodeDelegation",
                table: "delegations",
                newName: "ToUserCode");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "delegations",
                newName: "ToDate");

            migrationBuilder.RenameColumn(
                name: "FromOrgPositionId",
                table: "delegations",
                newName: "PositionId");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "delegations",
                newName: "FromDate");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "delegations",
                newName: "FromUserCode");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "delegations",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "IsPermanent",
                table: "delegations",
                type: "bit",
                nullable: true);
        }
    }
}
