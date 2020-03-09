using Microsoft.EntityFrameworkCore.Migrations;

namespace SwagOverflowWPF.Migrations
{
    public partial class M1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconString",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconTypeString",
                table: "SwagItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconString",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "IconTypeString",
                table: "SwagItems");
        }
    }
}
