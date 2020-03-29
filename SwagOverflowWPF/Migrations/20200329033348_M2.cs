using Microsoft.EntityFrameworkCore.Migrations;

namespace SwagOverflowWPF.Migrations
{
    public partial class M2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnsString",
                table: "SwagGroups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColumnsString",
                table: "SwagGroups",
                type: "TEXT",
                nullable: true);
        }
    }
}
