using Microsoft.EntityFrameworkCore.Migrations;

namespace SwagOverFlow.WPF.Migrations
{
    public partial class M0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SwagIndexedItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlternateId = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    Display = table.Column<string>(nullable: true),
                    IsExpanded = table.Column<bool>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    ValueTypeString = table.Column<string>(nullable: true),
                    ObjValue = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    IconString = table.Column<string>(nullable: true),
                    IconTypeString = table.Column<string>(nullable: true),
                    SettingType = table.Column<string>(nullable: true),
                    Value = table.Column<bool>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SwagSettingString_Value = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "SwagItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(nullable: true),
                    ValueTypeString = table.Column<string>(nullable: true),
                    AlternateId = table.Column<string>(nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    Display = table.Column<string>(nullable: true),
                    IsExpanded = table.Column<bool>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Columns = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwagItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_SwagItems_SwagItems_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SwagItems",
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

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_AlternateId",
                table: "SwagItems",
                column: "AlternateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_ParentId",
                table: "SwagItems",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SwagIndexedItems");

            migrationBuilder.DropTable(
                name: "SwagItems");
        }
    }
}
