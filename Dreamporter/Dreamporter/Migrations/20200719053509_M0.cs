using Microsoft.EntityFrameworkCore.Migrations;

namespace Dreamporter.Migrations
{
    public partial class M0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Condition = table.Column<string>(nullable: true),
                    RequiredData = table.Column<string>(nullable: true),
                    IntegrationId = table.Column<int>(nullable: true),
                    TestIntegrationId = table.Column<int>(nullable: true),
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
                });

            migrationBuilder.CreateTable(
                name: "Integrations",
                columns: table => new
                {
                    IntegrationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    BuildId = table.Column<int>(nullable: false),
                    TestBuildId = table.Column<int>(nullable: false),
                    InstructionTemplates = table.Column<string>(nullable: true),
                    DefaultOptions = table.Column<string>(nullable: true),
                    OptionsTree = table.Column<string>(nullable: true),
                    InitialSchemas = table.Column<string>(nullable: true),
                    DataContexts = table.Column<string>(nullable: true),
                    TestContexts = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integrations", x => x.IntegrationId);
                    table.ForeignKey(
                        name: "FK_Integrations_Builds_BuildId",
                        column: x => x.BuildId,
                        principalTable: "Builds",
                        principalColumn: "BuildId");
                    table.ForeignKey(
                        name: "FK_Integrations_Builds_TestBuildId",
                        column: x => x.TestBuildId,
                        principalTable: "Builds",
                        principalColumn: "BuildId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Builds_ParentId",
                table: "Builds",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_BuildId",
                table: "Integrations",
                column: "BuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_TestBuildId",
                table: "Integrations",
                column: "TestBuildId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Integrations");

            migrationBuilder.DropTable(
                name: "Builds");
        }
    }
}
