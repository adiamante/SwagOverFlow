using Microsoft.EntityFrameworkCore.Migrations;

namespace SwagOverflowWPF.Migrations
{
    public partial class M0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SwagGroups",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Display = table.Column<string>(nullable: true),
                    AlternateId = table.Column<string>(nullable: true),
                    RootId = table.Column<int>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwagGroups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "SwagItems",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlternateId = table.Column<string>(nullable: true),
                    GroupRootId = table.Column<int>(nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    Display = table.Column<string>(nullable: true),
                    IsExpanded = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    ValueTypeString = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    SettingType = table.Column<string>(nullable: true),
                    ItemsSource = table.Column<string>(nullable: true),
                    ItemsSourceTypeString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwagItems", x => new { x.GroupId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_SwagItems_SwagGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "SwagGroups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SwagItems_SwagGroups_GroupRootId",
                        column: x => x.GroupRootId,
                        principalTable: "SwagGroups",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_SwagItems_SwagItems_GroupId_ParentId",
                        columns: x => new { x.GroupId, x.ParentId },
                        principalTable: "SwagItems",
                        principalColumns: new[] { "GroupId", "ItemId" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_SwagGroups_AlternateId",
                table: "SwagGroups",
                column: "AlternateId",
                unique: true,
                filter: "[AlternateId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_AlternateId",
                table: "SwagItems",
                column: "AlternateId",
                unique: true,
                filter: "[AlternateId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_GroupRootId",
                table: "SwagItems",
                column: "GroupRootId",
                unique: true,
                filter: "[GroupRootId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_GroupId_ParentId",
                table: "SwagItems",
                columns: new[] { "GroupId", "ParentId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SwagItems");

            migrationBuilder.DropTable(
                name: "SwagGroups");
        }
    }
}
