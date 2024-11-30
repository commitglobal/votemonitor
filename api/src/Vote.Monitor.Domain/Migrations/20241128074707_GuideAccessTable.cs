using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class GuideAccessTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoalitionGuideAccess",
                columns: table => new
                {
                    CoalitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    GuideId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoalitionGuideAccess", x => new { x.CoalitionId, x.MonitoringNgoId, x.GuideId });
                    table.ForeignKey(
                        name: "FK_CoalitionGuideAccess_Coalitions_CoalitionId",
                        column: x => x.CoalitionId,
                        principalTable: "Coalitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoalitionGuideAccess_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoalitionGuideAccess_ObserversGuides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "ObserversGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoalitionGuideAccess_GuideId",
                table: "CoalitionGuideAccess",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_CoalitionGuideAccess_MonitoringNgoId",
                table: "CoalitionGuideAccess",
                column: "MonitoringNgoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoalitionGuideAccess");
        }
    }
}
