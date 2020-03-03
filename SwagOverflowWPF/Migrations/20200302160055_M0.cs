using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SwagOverflowWPF.Migrations
{
    public partial class M0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SwagItems",
                columns: table => new
                {
                    Group = table.Column<string>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(nullable: true),
                    Display = table.Column<string>(nullable: true),
                    Data = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwagItems", x => new { x.Group, x.Key, x.Id });
                    table.ForeignKey(
                        name: "FK_SwagItems_SwagItems_Group_Key_ParentId",
                        columns: x => new { x.Group, x.Key, x.ParentId },
                        principalTable: "SwagItems",
                        principalColumns: new[] { "Group", "Key", "Id" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_SwagItems_Group_Key_ParentId",
                table: "SwagItems",
                columns: new[] { "Group", "Key", "ParentId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SwagItems");
        }
    }
}
