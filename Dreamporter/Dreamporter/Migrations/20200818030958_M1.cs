using Microsoft.EntityFrameworkCore.Migrations;

namespace Dreamporter.Migrations
{
    public partial class M1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanUndo",
                table: "Builds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanUndo",
                table: "Builds",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
