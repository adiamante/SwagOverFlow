using Microsoft.EntityFrameworkCore.Migrations;

namespace Dreamporter.Migrations
{
    public partial class M0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Integrations",
                columns: table => new
                {
                    IntegrationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    BuildId = table.Column<int>(nullable: false),
                    InstructionTemplates = table.Column<string>(nullable: true),
                    DefaultOptions = table.Column<string>(nullable: true),
                    OptionsSet = table.Column<string>(nullable: true),
                    InitialSchemas = table.Column<string>(nullable: true),
                    DataContexts = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integrations", x => x.IntegrationId);
                });

            migrationBuilder.CreateTable(
                name: "Builds",
                columns: table => new
                {
                    BuildId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(nullable: false),
                    AlternateId = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    Display = table.Column<string>(nullable: true),
                    IsExpanded = table.Column<bool>(nullable: false),
                    IsSelected = table.Column<bool>(nullable: false),
                    CanUndo = table.Column<bool>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    IntegrationId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Condition = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    RequiredData = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Instructions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Builds", x => x.BuildId);
                    table.ForeignKey(
                        name: "FK_Builds_Builds_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Builds",
                        principalColumn: "BuildId");
                    table.ForeignKey(
                        name: "FK_Builds_Integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "Integrations",
                        principalColumn: "IntegrationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Builds_ParentId",
                table: "Builds",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Builds_IntegrationId",
                table: "Builds",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_BuildId",
                table: "Integrations",
                column: "BuildId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_Builds_BuildId",
                table: "Integrations",
                column: "BuildId",
                principalTable: "Builds",
                principalColumn: "BuildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Integrations_IntegrationId",
                table: "Builds");

            migrationBuilder.DropTable(
                name: "Integrations");

            migrationBuilder.DropTable(
                name: "Builds");
        }
    }
}
