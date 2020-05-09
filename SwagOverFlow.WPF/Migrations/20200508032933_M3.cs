using Microsoft.EntityFrameworkCore.Migrations;

namespace SwagOverflow.WPF.Migrations
{
    public partial class M3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Listening",
                table: "SwagItems");

            migrationBuilder.RenameColumn(
                name: "SwagSettingGroup_Name",
                table: "SwagItems",
                newName: "SwagDataTable_Name");

            migrationBuilder.AddColumn<string>(
                name: "AppliedFilter",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColumnName",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Expression",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Header",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedFilter",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedVisibility",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsColumnFilterOpen",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReadOnly",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ListCheckAll",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ListValuesFilter",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListValuesFilterMode",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReadOnly",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SearchFilter",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SearchFilterMode",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowAllDistinctValues",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "SwagItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "SwagItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliedFilter",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "ColumnName",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "Expression",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "Header",
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
                name: "IsReadOnly",
                table: "SwagItems");

            migrationBuilder.DropColumn(
                name: "IsVisible",
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
                name: "ReadOnly",
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
                name: "IsSelected",
                table: "SwagItems");

            migrationBuilder.RenameColumn(
                name: "SwagDataTable_Name",
                table: "SwagItems",
                newName: "SwagSettingGroup_Name");

            migrationBuilder.AddColumn<bool>(
                name: "Listening",
                table: "SwagItems",
                type: "INTEGER",
                nullable: true);
        }
    }
}
