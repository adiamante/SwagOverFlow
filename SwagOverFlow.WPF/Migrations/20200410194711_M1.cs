using Microsoft.EntityFrameworkCore.Migrations;

namespace SwagOverFlow.WPF.Migrations
{
    public partial class M1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SwagIndexedItems");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "SwagItems");

            migrationBuilder.AddColumn<bool>(
                name: "Listening",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SettingsItemId",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TabsItemId",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanUndo",
                table: "SwagItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjValue",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconString",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconTypeString",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemsSourceTypeString",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjItemsSource",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SwagSetting_ParentId",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SettingType",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwagSettingGroup_Name",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInitialized",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelectedIndex",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowChildText",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwagTabItem_IconString",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwagTabItem_IconTypeString",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SwagTabItem_ParentId",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_SettingsItemId",
                table: "SwagItems",
                column: "SettingsItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_TabsItemId",
                table: "SwagItems",
                column: "TabsItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_SwagSetting_ParentId",
                table: "SwagItems",
                column: "SwagSetting_ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_SwagTabItem_ParentId",
                table: "SwagItems",
                column: "SwagTabItem_ParentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SwagItems_SwagItems_SwagSetting_ParentId",
                table: "SwagItems",
                column: "SwagSetting_ParentId",
                principalTable: "SwagItems",
                principalColumn: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_SwagItems_SwagItems_SwagTabItem_ParentId",
                table: "SwagItems",
                column: "SwagTabItem_ParentId",
                principalTable: "SwagItems",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwagItems_SwagItems_SettingsItemId",
                table: "SwagItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SwagItems_SwagItems_TabsItemId",
                table: "SwagItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SwagItems_SwagItems_SwagSetting_ParentId",
                table: "SwagItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SwagItems_SwagItems_SwagTabItem_ParentId",
                table: "SwagItems");

            migrationBuilder.DropIndex(
                name: "IX_SwagItems_SettingsItemId",
                table: "SwagItems");

            migrationBuilder.DropIndex(
                name: "IX_SwagItems_TabsItemId",
                table: "SwagItems");

            migrationBuilder.DropIndex(
                name: "IX_SwagItems_SwagSetting_ParentId",
                table: "SwagItems");

            migrationBuilder.DropIndex(
                name: "IX_SwagItems_SwagTabItem_ParentId",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "Listening",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SettingsItemId",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "TabsItemId",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "CanUndo",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "ObjValue",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "IconString",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "IconTypeString",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "ItemsSourceTypeString",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "ObjItemsSource",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SwagSetting_ParentId",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SettingType",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SwagSettingGroup_Name",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "IsInitialized",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SelectedIndex",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "ShowChildText",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SwagTabItem_IconString",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SwagTabItem_IconTypeString",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SwagTabItem_ParentId",
                table: "SwagItems");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "SwagItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SwagIndexedItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlternateId = table.Column<string>(type: "TEXT", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false),
                    Display = table.Column<string>(type: "TEXT", nullable: true),
                    IsExpanded = table.Column<bool>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: true),
                    ObjValue = table.Column<string>(type: "TEXT", nullable: true),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    ValueTypeString = table.Column<string>(type: "TEXT", nullable: true),
                    IconString = table.Column<string>(type: "TEXT", nullable: true),
                    IconTypeString = table.Column<string>(type: "TEXT", nullable: true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    SettingType = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<bool>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    SwagSettingString_Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwagIndexedItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_SwagIndexedItems_SwagIndexedItems_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SwagIndexedItems",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SwagIndexedItems_AlternateId",
                table: "SwagIndexedItems",
                column: "AlternateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SwagIndexedItems_ParentId",
                table: "SwagIndexedItems",
                column: "ParentId");
        }
    }
}
