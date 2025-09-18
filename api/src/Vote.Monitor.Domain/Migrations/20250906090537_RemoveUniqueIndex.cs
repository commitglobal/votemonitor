using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_ElectionRoundId_PollingStationId_Monitoring~",
                table: "FormSubmissions");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_ElectionRoundId_PollingStationId_Monitoring~",
                table: "FormSubmissions",
                columns: new[] { "ElectionRoundId", "PollingStationId", "MonitoringObserverId", "FormId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_ElectionRoundId_PollingStationId_Monitoring~",
                table: "FormSubmissions");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_ElectionRoundId_PollingStationId_Monitoring~",
                table: "FormSubmissions",
                columns: new[] { "ElectionRoundId", "PollingStationId", "MonitoringObserverId", "FormId" },
                unique: true);
        }
    }
}
