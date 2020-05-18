using Microsoft.EntityFrameworkCore.Migrations;

namespace SwagOverFlow.WPF.Migrations
{
    public partial class M4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwagItems_SwagItems_ParentId",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "IsCheckedFilter",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "IsCheckedVisibility",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "IsColumnFilterOpen",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "ListCheckAll",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "ListValuesFilter",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "ListValuesFilterMode",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SearchFilter",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "SearchFilterMode",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "ShowAllDistinctValues",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "Columns",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "Settings",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "Tabs",
                table: "SwagItems");

            migrationBuilder.RenameColumn(
                name: "SwagDataTable_Name",
                table: "SwagItems",
                newName: "SwagSettingGroup_Name");

            migrationBuilder.AddColumn<int>(
                name: "ColSeq",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataTypeString",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SwagItems_SwagItems_ParentId",
                table: "SwagItems",
                column: "ParentId",
                principalTable: "SwagItems",
                principalColumn: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwagItems_SwagItems_ParentId",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "ColSeq",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "DataTypeString",
                table: "SwagItems");

            migrationBuilder.RenameColumn(
                name: "SwagSettingGroup_Name",
                table: "SwagItems",
                newName: "SwagDataTable_Name");

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedFilter",
                table: "SwagItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedVisibility",
                table: "SwagItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsColumnFilterOpen",
                table: "SwagItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ListCheckAll",
                table: "SwagItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ListValuesFilter",
                table: "SwagItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListValuesFilterMode",
                table: "SwagItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SearchFilter",
                table: "SwagItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SearchFilterMode",
                table: "SwagItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowAllDistinctValues",
                table: "SwagItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "SwagItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Columns",
                table: "SwagItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Settings",
                table: "SwagItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tabs",
                table: "SwagItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SwagItems_SwagItems_ParentId",
                table: "SwagItems",
                column: "ParentId",
                principalTable: "SwagItems",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
