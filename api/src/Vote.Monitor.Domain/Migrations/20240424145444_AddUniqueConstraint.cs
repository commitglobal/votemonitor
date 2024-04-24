using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PollingStationInformation_ElectionRoundId",
                table: "PollingStationInformation");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformation_ElectionRoundId_PollingStationId_~",
                table: "PollingStationInformation",
                columns: new[] { "ElectionRoundId", "PollingStationId", "MonitoringObserverId", "PollingStationInformationFormId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PollingStationInformation_ElectionRoundId_PollingStationId_~",
                table: "PollingStationInformation");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformation_ElectionRoundId",
                table: "PollingStationInformation",
                column: "ElectionRoundId");
        }
    }
}
