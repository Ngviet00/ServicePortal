using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAttachFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attach_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attach_files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "attach_file_relations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RefId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RefType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attach_file_relations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attach_file_relations_attach_files_AttachFileId",
                        column: x => x.AttachFileId,
                        principalTable: "attach_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attach_file_relations_AttachFileId_RefId_RefType",
                table: "attach_file_relations",
                columns: new[] { "AttachFileId", "RefId", "RefType" });

            migrationBuilder.CreateIndex(
                name: "IX_attach_files_Id",
                table: "attach_files",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attach_file_relations");

            migrationBuilder.DropTable(
                name: "attach_files");
        }
    }
}
