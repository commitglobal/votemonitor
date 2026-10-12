using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddPollingStationCorrections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PollingStationCorrections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordType = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromPollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToPollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CorrectedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationCorrections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationCorrections_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationCorrections_PollingStations_FromPollingStatio~",
                        column: x => x.FromPollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationCorrections_PollingStations_ToPollingStationId",
                        column: x => x.ToPollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationCorrections_ElectionRoundId_RecordType_Record~",
                table: "PollingStationCorrections",
                columns: new[] { "ElectionRoundId", "RecordType", "RecordId" });

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationCorrections_FromPollingStationId",
                table: "PollingStationCorrections",
                column: "FromPollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationCorrections_ToPollingStationId",
                table: "PollingStationCorrections",
                column: "ToPollingStationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollingStationCorrections");
        }
    }
}
