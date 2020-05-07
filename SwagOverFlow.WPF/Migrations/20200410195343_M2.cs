using Microsoft.EntityFrameworkCore.Migrations;

namespace SwagOverflow.WPF.Migrations
{
    public partial class M2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwagItems_SwagItems_SettingsItemId",
                table: "SwagItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SwagItems_SwagItems_TabsItemId",
                table: "SwagItems");

            migrationBuilder.DropIndex(
                name: "IX_SwagItems_SettingsItemId",
                table: "SwagItems");

            migrationBuilder.DropIndex(
                name: "IX_SwagItems_TabsItemId",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SettingsItemId",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "TabsItemId",
                table: "SwagItems");

            migrationBuilder.AddColumn<string>(
                name: "Settings",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tabs",
                table: "SwagItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Settings",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "Tabs",
                table: "SwagItems");

            migrationBuilder.AddColumn<int>(
                name: "SettingsItemId",
                table: "SwagItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TabsItemId",
                table: "SwagItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_SettingsItemId",
                table: "SwagItems",
                column: "SettingsItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_TabsItemId",
                table: "SwagItems",
                column: "TabsItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_SwagItems_SwagItems_SettingsItemId",
                table: "SwagItems",
                column: "SettingsItemId",
                principalTable: "SwagItems",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SwagItems_SwagItems_TabsItemId",
                table: "SwagItems",
                column: "TabsItemId",
                principalTable: "SwagItems",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
