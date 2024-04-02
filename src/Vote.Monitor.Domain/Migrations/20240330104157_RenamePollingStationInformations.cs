using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenamePollingStationInformations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationInformations_ElectionRounds_ElectionRoundId",
                table: "PollingStationInformations");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationInformations_MonitoringObservers_MonitoringOb~",
                table: "PollingStationInformations");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationInformations_PollingStationInformationForms_P~",
                table: "PollingStationInformations");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationInformations_PollingStations_PollingStationId",
                table: "PollingStationInformations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PollingStationInformations",
                table: "PollingStationInformations");

            migrationBuilder.RenameTable(
                name: "PollingStationInformations",
                newName: "PollingStationInformation");

            migrationBuilder.RenameIndex(
                name: "IX_PollingStationInformations_PollingStationInformationFormId",
                table: "PollingStationInformation",
                newName: "IX_PollingStationInformation_PollingStationInformationFormId");

            migrationBuilder.RenameIndex(
                name: "IX_PollingStationInformations_PollingStationId",
                table: "PollingStationInformation",
                newName: "IX_PollingStationInformation_PollingStationId");

            migrationBuilder.RenameIndex(
                name: "IX_PollingStationInformations_MonitoringObserverId",
                table: "PollingStationInformation",
                newName: "IX_PollingStationInformation_MonitoringObserverId");

            migrationBuilder.RenameIndex(
                name: "IX_PollingStationInformations_ElectionRoundId",
                table: "PollingStationInformation",
                newName: "IX_PollingStationInformation_ElectionRoundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PollingStationInformation",
                table: "PollingStationInformation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationInformation_ElectionRounds_ElectionRoundId",
                table: "PollingStationInformation",
                column: "ElectionRoundId",
                principalTable: "ElectionRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationInformation_MonitoringObservers_MonitoringObs~",
                table: "PollingStationInformation",
                column: "MonitoringObserverId",
                principalTable: "MonitoringObservers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationInformation_PollingStationInformationForms_Po~",
                table: "PollingStationInformation",
                column: "PollingStationInformationFormId",
                principalTable: "PollingStationInformationForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationInformation_PollingStations_PollingStationId",
                table: "PollingStationInformation",
                column: "PollingStationId",
                principalTable: "PollingStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationInformation_ElectionRounds_ElectionRoundId",
                table: "PollingStationInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationInformation_MonitoringObservers_MonitoringObs~",
                table: "PollingStationInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationInformation_PollingStationInformationForms_Po~",
                table: "PollingStationInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationInformation_PollingStations_PollingStationId",
                table: "PollingStationInformation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PollingStationInformation",
                table: "PollingStationInformation");

            migrationBuilder.RenameTable(
                name: "PollingStationInformation",
                newName: "PollingStationInformations");

            migrationBuilder.RenameIndex(
                name: "IX_PollingStationInformation_PollingStationInformationFormId",
                table: "PollingStationInformations",
                newName: "IX_PollingStationInformations_PollingStationInformationFormId");

            migrationBuilder.RenameIndex(
                name: "IX_PollingStationInformation_PollingStationId",
                table: "PollingStationInformations",
                newName: "IX_PollingStationInformations_PollingStationId");

            migrationBuilder.RenameIndex(
                name: "IX_PollingStationInformation_MonitoringObserverId",
                table: "PollingStationInformations",
                newName: "IX_PollingStationInformations_MonitoringObserverId");

            migrationBuilder.RenameIndex(
                name: "IX_PollingStationInformation_ElectionRoundId",
                table: "PollingStationInformations",
                newName: "IX_PollingStationInformations_ElectionRoundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PollingStationInformations",
                table: "PollingStationInformations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationInformations_ElectionRounds_ElectionRoundId",
                table: "PollingStationInformations",
                column: "ElectionRoundId",
                principalTable: "ElectionRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationInformations_MonitoringObservers_MonitoringOb~",
                table: "PollingStationInformations",
                column: "MonitoringObserverId",
                principalTable: "MonitoringObservers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationInformations_PollingStationInformationForms_P~",
                table: "PollingStationInformations",
                column: "PollingStationInformationFormId",
                principalTable: "PollingStationInformationForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationInformations_PollingStations_PollingStationId",
                table: "PollingStationInformations",
                column: "PollingStationId",
                principalTable: "PollingStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
