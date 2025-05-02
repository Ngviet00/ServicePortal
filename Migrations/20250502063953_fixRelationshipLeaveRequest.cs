using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class fixRelationshipLeaveRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leave_request_steps_leave_requests_leave_request_id",
                table: "leave_request_steps");

            migrationBuilder.AddColumn<Guid>(
                name: "leave_request_id1",
                table: "leave_request_steps",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_leave_request_steps_leave_request_id1",
                table: "leave_request_steps",
                column: "leave_request_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_leave_request_steps_leave_requests_leave_request_id",
                table: "leave_request_steps",
                column: "leave_request_id",
                principalTable: "leave_requests",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_leave_request_steps_leave_requests_leave_request_id1",
                table: "leave_request_steps",
                column: "leave_request_id1",
                principalTable: "leave_requests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leave_request_steps_leave_requests_leave_request_id",
                table: "leave_request_steps");

            migrationBuilder.DropForeignKey(
                name: "FK_leave_request_steps_leave_requests_leave_request_id1",
                table: "leave_request_steps");

            migrationBuilder.DropIndex(
                name: "IX_leave_request_steps_leave_request_id1",
                table: "leave_request_steps");

            migrationBuilder.DropColumn(
                name: "leave_request_id1",
                table: "leave_request_steps");

            migrationBuilder.AddForeignKey(
                name: "FK_leave_request_steps_leave_requests_leave_request_id",
                table: "leave_request_steps",
                column: "leave_request_id",
                principalTable: "leave_requests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
