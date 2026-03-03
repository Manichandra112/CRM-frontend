using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM_Backend.Migrations
{
    /// <inheritdoc />
    public partial class MakeModuleIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "module",
                table: "permissions");

            migrationBuilder.AlterColumn<string>(
                name: "account_status",
                table: "users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<long>(
                name: "module_id",
                table: "roles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "module_id",
                table: "permissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "modules",
                columns: table => new
                {
                    module_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    module_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    module_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modules", x => x.module_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_roles_module_id",
                table: "roles",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_module_id",
                table: "permissions",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_modules_module_code",
                table: "modules",
                column: "module_code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_permissions_modules_module_id",
                table: "permissions",
                column: "module_id",
                principalTable: "modules",
                principalColumn: "module_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_roles_modules_module_id",
                table: "roles",
                column: "module_id",
                principalTable: "modules",
                principalColumn: "module_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_permissions_modules_module_id",
                table: "permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_roles_modules_module_id",
                table: "roles");

            migrationBuilder.DropTable(
                name: "modules");

            migrationBuilder.DropIndex(
                name: "IX_roles_module_id",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "IX_permissions_module_id",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "module_id",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "module_id",
                table: "permissions");

            migrationBuilder.AlterColumn<string>(
                name: "account_status",
                table: "users",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "module",
                table: "permissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
