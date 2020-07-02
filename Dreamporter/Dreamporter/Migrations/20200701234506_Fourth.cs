using Microsoft.EntityFrameworkCore.Migrations;

namespace Dreamporter.Migrations
{
    public partial class Fourth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoreBuild",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "ProfileBuild",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "SelectedOptions",
                table: "Integrations");

            migrationBuilder.AddColumn<int>(
                name: "CoreBuildId",
                table: "Integrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProfileBuildId",
                table: "Integrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BaseBuilds",
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
                    IsEnabled = table.Column<bool>(nullable: false),
                    RequiredData = table.Column<string>(nullable: true),
                    Instructions = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Condition = table.Column<string>(nullable: true),
                    InstructionTemplateGUID = table.Column<string>(nullable: true),
                    Options = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseBuilds", x => x.BuildId);
                    table.ForeignKey(
                        name: "FK_BaseBuilds_BaseBuilds_ParentId",
                        column: x => x.ParentId,
                        principalTable: "BaseBuilds",
                        principalColumn: "BuildId");
                    table.ForeignKey(
                        name: "FK_BaseBuilds_Integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "Integrations",
                        principalColumn: "IntegrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseBuilds_Integrations_IntegrationId1",
                        column: x => x.IntegrationId,
                        principalTable: "Integrations",
                        principalColumn: "IntegrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseBuilds_Integrations_IntegrationId2",
                        column: x => x.IntegrationId,
                        principalTable: "Integrations",
                        principalColumn: "IntegrationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseBuilds_ParentId",
                table: "BaseBuilds",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseBuilds_IntegrationId",
                table: "BaseBuilds",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseBuilds_IntegrationId1",
                table: "BaseBuilds",
                column: "IntegrationId",
                unique: true,
                filter: "[IntegrationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BaseBuilds_IntegrationId2",
                table: "BaseBuilds",
                column: "IntegrationId",
                unique: true,
                filter: "[IntegrationId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseBuilds");

            migrationBuilder.DropColumn(
                name: "CoreBuildId",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "ProfileBuildId",
                table: "Integrations");

            migrationBuilder.AddColumn<string>(
                name: "CoreBuild",
                table: "Integrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileBuild",
                table: "Integrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedOptions",
                table: "Integrations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
