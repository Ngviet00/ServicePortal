using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeColumnIdTblPurchaseItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
            name: "PK_purchase_details",
            table: "purchase_details");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "purchase_details");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "purchase_details",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_purchase_details",
                table: "purchase_details",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
            name: "PK_purchase_details",
            table: "purchase_details");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "purchase_details");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "purchase_details",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_purchase_details",
                table: "purchase_details",
                column: "Id");
        }
    }
}
