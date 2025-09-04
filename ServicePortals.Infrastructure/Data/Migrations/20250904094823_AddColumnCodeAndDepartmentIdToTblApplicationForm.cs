using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnCodeAndDepartmentIdToTblApplicationForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_it_forms_application_forms_ApplicationFormId",
                table: "it_forms");

            migrationBuilder.DropForeignKey(
                name: "FK_it_forms_users_UserCodeCreated",
                table: "it_forms");

            migrationBuilder.DropForeignKey(
                name: "FK_leave_requests_users_UserCodeRequestor",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "IX_leave_requests_Id_UserCodeRequestor",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "IX_leave_requests_UserCodeRequestor",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "IX_it_forms_ApplicationFormId",
                table: "it_forms");

            migrationBuilder.DropIndex(
                name: "IX_it_forms_UserCodeCreated",
                table: "it_forms");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "UserCode",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "memo_notifications");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "memo_notifications");

            migrationBuilder.DropColumn(
                name: "UserCodeCreated",
                table: "memo_notifications");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "UserCodeCreated",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "UserCodeRequestor",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "UserNameRequestor",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "it_forms");

            migrationBuilder.DropColumn(
                name: "UserCodeCreated",
                table: "it_forms");

            migrationBuilder.DropColumn(
                name: "UserCodeRequestor",
                table: "it_forms");

            migrationBuilder.DropColumn(
                name: "UserNameCreated",
                table: "it_forms");

            migrationBuilder.DropColumn(
                name: "UserNameRequestor",
                table: "it_forms");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "leave_requests",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "leave_requests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationFormId",
                table: "it_forms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "application_forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "application_forms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaData",
                table: "application_forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Step",
                table: "application_forms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCodeCreated",
                table: "application_forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserNameCreated",
                table: "application_forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_it_forms_ApplicationFormId",
                table: "it_forms",
                column: "ApplicationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_Id",
                table: "leave_requests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_UserId",
                table: "leave_requests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_application_forms_DepartmentId",
                table: "application_forms",
                column: "DepartmentId",
                unique: true,
                filter: "[DepartmentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_application_forms_it_forms_Id",
                table: "application_forms",
                column: "Id",
                principalTable: "it_forms",
                principalColumn: "ApplicationFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_application_forms_org_units_DepartmentId",
                table: "application_forms",
                column: "DepartmentId",
                principalTable: "org_units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_leave_requests_users_UserId",
                table: "leave_requests",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_forms_it_forms_Id",
                table: "application_forms");

            migrationBuilder.DropForeignKey(
                name: "FK_application_forms_org_units_DepartmentId",
                table: "application_forms");

            migrationBuilder.DropForeignKey(
                name: "FK_leave_requests_users_UserId",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "IX_leave_requests_Id",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "IX_leave_requests_UserId",
                table: "leave_requests");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_it_forms_ApplicationFormId",
                table: "it_forms");

            migrationBuilder.DropIndex(
                name: "IX_application_forms_DepartmentId",
                table: "application_forms");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "application_forms");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "application_forms");

            migrationBuilder.DropColumn(
                name: "MetaData",
                table: "application_forms");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "application_forms");

            migrationBuilder.DropColumn(
                name: "UserCodeCreated",
                table: "application_forms");

            migrationBuilder.DropColumn(
                name: "UserNameCreated",
                table: "application_forms");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "purchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCode",
                table: "purchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "purchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "memo_notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "memo_notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCodeCreated",
                table: "memo_notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "leave_requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "leave_requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCodeCreated",
                table: "leave_requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCodeRequestor",
                table: "leave_requests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserNameRequestor",
                table: "leave_requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationFormId",
                table: "it_forms",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "it_forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCodeCreated",
                table: "it_forms",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCodeRequestor",
                table: "it_forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserNameCreated",
                table: "it_forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserNameRequestor",
                table: "it_forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_Id_UserCodeRequestor",
                table: "leave_requests",
                columns: new[] { "Id", "UserCodeRequestor" });

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_UserCodeRequestor",
                table: "leave_requests",
                column: "UserCodeRequestor");

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_ApplicationFormId",
                table: "it_forms",
                column: "ApplicationFormId",
                unique: true,
                filter: "[ApplicationFormId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_UserCodeCreated",
                table: "it_forms",
                column: "UserCodeCreated");

            migrationBuilder.AddForeignKey(
                name: "FK_it_forms_application_forms_ApplicationFormId",
                table: "it_forms",
                column: "ApplicationFormId",
                principalTable: "application_forms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_it_forms_users_UserCodeCreated",
                table: "it_forms",
                column: "UserCodeCreated",
                principalTable: "users",
                principalColumn: "UserCode");

            migrationBuilder.AddForeignKey(
                name: "FK_leave_requests_users_UserCodeRequestor",
                table: "leave_requests",
                column: "UserCodeRequestor",
                principalTable: "users",
                principalColumn: "UserCode");
        }
    }
}
