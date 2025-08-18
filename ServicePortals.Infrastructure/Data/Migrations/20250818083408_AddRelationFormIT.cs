using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicePortals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationFormIT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "it_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_it_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "priorities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "it_forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserCodeRequestor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserCodeCreated = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriorityId = table.Column<int>(type: "int", nullable: true),
                    NoteManagerIT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RequiredCompletionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TargetCompletionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActualCompletionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_it_forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "it_form_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ITFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ITCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_it_form_categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "it_categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, null, "Server Login Id" },
                    { 2, null, "Network device" },
                    { 3, null, "Email" },
                    { 4, null, "Software Installation" },
                    { 5, null, "ERP Login Id" },
                    { 6, null, "Other" }
                });

            migrationBuilder.InsertData(
                table: "priorities",
                columns: new[] { "Id", "Name", "NameE" },
                values: new object[,]
                {
                    { 1, "Thấp", "Low" },
                    { 2, "Trung bình", "Medium" },
                    { 3, "Cao", "High" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_it_form_categories_ITCategoryId",
                table: "it_form_categories",
                column: "ITCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_it_form_categories_ITFormId",
                table: "it_form_categories",
                column: "ITFormId");

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_ApplicationFormId",
                table: "it_forms",
                column: "ApplicationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_DepartmentId",
                table: "it_forms",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_PriorityId",
                table: "it_forms",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_it_forms_UserCodeCreated",
                table: "it_forms",
                column: "UserCodeCreated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "it_form_categories");

            migrationBuilder.DropTable(
                name: "it_categories");

            migrationBuilder.DropTable(
                name: "it_forms");

            migrationBuilder.DropTable(
                name: "priorities");
        }
    }
}
