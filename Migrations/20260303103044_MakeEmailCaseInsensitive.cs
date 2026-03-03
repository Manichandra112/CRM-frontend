using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Backend.Migrations
{
    public partial class MakeEmailCaseInsensitive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Enable citext extension (safe if already exists)
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS citext;");

            // Change email column to citext
            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "citext",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert back to varchar if rolled back
            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");
        }
    }
}