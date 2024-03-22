using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddPollingStationInformation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PollingStationInformations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationInformationFormId = table.Column<Guid>(type: "uuid", nullable: false),
                    Answers = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationInformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_MonitoringObserver_MonitoringObs~",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObserver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_PollingStationInformationForms_P~",
                        column: x => x.PollingStationInformationFormId,
                        principalTable: "PollingStationInformationForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_ElectionRoundId",
                table: "PollingStationInformations",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_MonitoringObserverId",
                table: "PollingStationInformations",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_PollingStationId",
                table: "PollingStationInformations",
                column: "PollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_PollingStationInformationFormId",
                table: "PollingStationInformations",
                column: "PollingStationInformationFormId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollingStationInformations");
        }
    }
}
