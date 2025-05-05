using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePortal.Migrations
{
    /// <inheritdoc />
    public partial class addCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_permissions_permission_id",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_roles_role_id",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_permissions_permissions_permission_id",
                table: "user_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_permissions_users_user_code",
                table: "user_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_user_code",
                table: "user_roles");

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id",
                principalTable: "permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_roles_role_id",
                table: "role_permissions",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_permissions_permissions_permission_id",
                table: "user_permissions",
                column: "permission_id",
                principalTable: "permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_permissions_users_user_code",
                table: "user_permissions",
                column: "user_code",
                principalTable: "users",
                principalColumn: "code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_user_code",
                table: "user_roles",
                column: "user_code",
                principalTable: "users",
                principalColumn: "code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_permissions_permission_id",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_roles_role_id",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_permissions_permissions_permission_id",
                table: "user_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_permissions_users_user_code",
                table: "user_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_user_code",
                table: "user_roles");

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id",
                principalTable: "permissions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_roles_role_id",
                table: "role_permissions",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_permissions_permissions_permission_id",
                table: "user_permissions",
                column: "permission_id",
                principalTable: "permissions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_permissions_users_user_code",
                table: "user_permissions",
                column: "user_code",
                principalTable: "users",
                principalColumn: "code");

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_user_code",
                table: "user_roles",
                column: "user_code",
                principalTable: "users",
                principalColumn: "code");
        }
    }
}
