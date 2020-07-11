using Microsoft.EntityFrameworkCore.Migrations;

namespace Dreamporter.Migrations
{
    public partial class M1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrations_Builds_BuildId",
                table: "Integrations");

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_Builds_BuildId",
                table: "Integrations",
                column: "BuildId",
                principalTable: "Builds",
                principalColumn: "BuildId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrations_Builds_BuildId",
                table: "Integrations");

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_Builds_BuildId",
                table: "Integrations",
                column: "BuildId",
                principalTable: "Builds",
                principalColumn: "BuildId");
        }
    }
}
