using Microsoft.EntityFrameworkCore.Migrations;

namespace Dreamporter.Migrations
{
    public partial class M2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Integrations_IntegrationId",
                table: "Builds");

            migrationBuilder.DropIndex(
                name: "IX_Builds_IntegrationId",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "OptionsSet",
                table: "Integrations");

            migrationBuilder.AddColumn<string>(
                name: "OptionsTree",
                table: "Integrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionsTree",
                table: "Integrations");

            migrationBuilder.AddColumn<string>(
                name: "OptionsSet",
                table: "Integrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Builds_IntegrationId",
                table: "Builds",
                column: "IntegrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Integrations_IntegrationId",
                table: "Builds",
                column: "IntegrationId",
                principalTable: "Integrations",
                principalColumn: "IntegrationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
