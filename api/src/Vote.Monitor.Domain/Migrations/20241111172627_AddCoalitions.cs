using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddCoalitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coalitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LeaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coalitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coalitions_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Coalitions_MonitoringNgos_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoalitionFormAccess",
                columns: table => new
                {
                    CoalitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoalitionFormAccess", x => new { x.CoalitionId, x.MonitoringNgoId, x.FormId });
                    table.ForeignKey(
                        name: "FK_CoalitionFormAccess_Coalitions_CoalitionId",
                        column: x => x.CoalitionId,
                        principalTable: "Coalitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoalitionFormAccess_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoalitionFormAccess_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoalitionMemberships",
                columns: table => new
                {
                    CoalitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoalitionMemberships", x => new { x.MonitoringNgoId, x.CoalitionId });
                    table.ForeignKey(
                        name: "FK_CoalitionMemberships_Coalitions_CoalitionId",
                        column: x => x.CoalitionId,
                        principalTable: "Coalitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoalitionMemberships_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoalitionMemberships_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoalitionFormAccess_FormId",
                table: "CoalitionFormAccess",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_CoalitionFormAccess_MonitoringNgoId",
                table: "CoalitionFormAccess",
                column: "MonitoringNgoId");

            migrationBuilder.CreateIndex(
                name: "IX_CoalitionMemberships_CoalitionId",
                table: "CoalitionMemberships",
                column: "CoalitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CoalitionMemberships_ElectionRoundId",
                table: "CoalitionMemberships",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CoalitionMemberships_MonitoringNgoId_CoalitionId_ElectionRo~",
                table: "CoalitionMemberships",
                columns: new[] { "MonitoringNgoId", "CoalitionId", "ElectionRoundId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoalitionMemberships_MonitoringNgoId_ElectionRoundId",
                table: "CoalitionMemberships",
                columns: new[] { "MonitoringNgoId", "ElectionRoundId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coalitions_ElectionRoundId",
                table: "Coalitions",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Coalitions_LeaderId",
                table: "Coalitions",
                column: "LeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoalitionFormAccess");

            migrationBuilder.DropTable(
                name: "CoalitionMemberships");

            migrationBuilder.DropTable(
                name: "Coalitions");
        }
    }
}
