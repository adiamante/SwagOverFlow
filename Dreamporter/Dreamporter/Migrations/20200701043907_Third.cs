using Microsoft.EntityFrameworkCore.Migrations;

namespace Dreamporter.Migrations
{
    public partial class Third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DataContexts",
                table: "Integrations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultOptions",
                table: "Integrations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InitialSchemas",
                table: "Integrations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructionTemplates",
                table: "Integrations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionsSet",
                table: "Integrations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileBuild",
                table: "Integrations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedOptions",
                table: "Integrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataContexts",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "DefaultOptions",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "InitialSchemas",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "InstructionTemplates",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "OptionsSet",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "ProfileBuild",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "SelectedOptions",
                table: "Integrations");
        }
    }
}
