using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Missing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringNgo_ElectionRounds_ElectionRoundId",
                table: "MonitoringNgo");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringNgo_Ngos_NgoId",
                table: "MonitoringNgo");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObserver_MonitoringNgo_InviterNgoId",
                table: "MonitoringObserver");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObserver_Observers_ObserverId",
                table: "MonitoringObserver");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObserverNotification_MonitoringObserver_TargetedO~",
                table: "MonitoringObserverNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationAttachment_MonitoringObserver_MonitoringObser~",
                table: "PollingStationAttachment");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationInformations_MonitoringObserver_MonitoringObs~",
                table: "PollingStationInformations");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationNote_MonitoringObserver_MonitoringObserverId",
                table: "PollingStationNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonitoringObserver",
                table: "MonitoringObserver");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonitoringNgo",
                table: "MonitoringNgo");

            migrationBuilder.RenameTable(
                name: "MonitoringObserver",
                newName: "MonitoringObservers");

            migrationBuilder.RenameTable(
                name: "MonitoringNgo",
                newName: "MonitoringNgos");

            migrationBuilder.RenameIndex(
                name: "IX_MonitoringObserver_ObserverId",
                table: "MonitoringObservers",
                newName: "IX_MonitoringObservers_ObserverId");

            migrationBuilder.RenameIndex(
                name: "IX_MonitoringObserver_InviterNgoId",
                table: "MonitoringObservers",
                newName: "IX_MonitoringObservers_InviterNgoId");

            migrationBuilder.RenameIndex(
                name: "IX_MonitoringNgo_NgoId",
                table: "MonitoringNgos",
                newName: "IX_MonitoringNgos_NgoId");

            migrationBuilder.RenameIndex(
                name: "IX_MonitoringNgo_ElectionRoundId",
                table: "MonitoringNgos",
                newName: "IX_MonitoringNgos_ElectionRoundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonitoringObservers",
                table: "MonitoringObservers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonitoringNgos",
                table: "MonitoringNgos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringNgos_ElectionRounds_ElectionRoundId",
                table: "MonitoringNgos",
                column: "ElectionRoundId",
                principalTable: "ElectionRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringNgos_Ngos_NgoId",
                table: "MonitoringNgos",
                column: "NgoId",
                principalTable: "Ngos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObserverNotification_MonitoringObservers_Targeted~",
                table: "MonitoringObserverNotification",
                column: "TargetedObserversId",
                principalTable: "MonitoringObservers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObservers_MonitoringNgos_InviterNgoId",
                table: "MonitoringObservers",
                column: "InviterNgoId",
                principalTable: "MonitoringNgos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObservers_Observers_ObserverId",
                table: "MonitoringObservers",
                column: "ObserverId",
                principalTable: "Observers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationAttachment_MonitoringObservers_MonitoringObse~",
                table: "PollingStationAttachment",
                column: "MonitoringObserverId",
                principalTable: "MonitoringObservers",
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
                name: "FK_PollingStationNote_MonitoringObservers_MonitoringObserverId",
                table: "PollingStationNote",
                column: "MonitoringObserverId",
                principalTable: "MonitoringObservers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringNgos_ElectionRounds_ElectionRoundId",
                table: "MonitoringNgos");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringNgos_Ngos_NgoId",
                table: "MonitoringNgos");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObserverNotification_MonitoringObservers_Targeted~",
                table: "MonitoringObserverNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObservers_MonitoringNgos_InviterNgoId",
                table: "MonitoringObservers");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObservers_Observers_ObserverId",
                table: "MonitoringObservers");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationAttachment_MonitoringObservers_MonitoringObse~",
                table: "PollingStationAttachment");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationInformations_MonitoringObservers_MonitoringOb~",
                table: "PollingStationInformations");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationNote_MonitoringObservers_MonitoringObserverId",
                table: "PollingStationNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonitoringObservers",
                table: "MonitoringObservers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonitoringNgos",
                table: "MonitoringNgos");

            migrationBuilder.RenameTable(
                name: "MonitoringObservers",
                newName: "MonitoringObserver");

            migrationBuilder.RenameTable(
                name: "MonitoringNgos",
                newName: "MonitoringNgo");

            migrationBuilder.RenameIndex(
                name: "IX_MonitoringObservers_ObserverId",
                table: "MonitoringObserver",
                newName: "IX_MonitoringObserver_ObserverId");

            migrationBuilder.RenameIndex(
                name: "IX_MonitoringObservers_InviterNgoId",
                table: "MonitoringObserver",
                newName: "IX_MonitoringObserver_InviterNgoId");

            migrationBuilder.RenameIndex(
                name: "IX_MonitoringNgos_NgoId",
                table: "MonitoringNgo",
                newName: "IX_MonitoringNgo_NgoId");

            migrationBuilder.RenameIndex(
                name: "IX_MonitoringNgos_ElectionRoundId",
                table: "MonitoringNgo",
                newName: "IX_MonitoringNgo_ElectionRoundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonitoringObserver",
                table: "MonitoringObserver",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonitoringNgo",
                table: "MonitoringNgo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringNgo_ElectionRounds_ElectionRoundId",
                table: "MonitoringNgo",
                column: "ElectionRoundId",
                principalTable: "ElectionRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringNgo_Ngos_NgoId",
                table: "MonitoringNgo",
                column: "NgoId",
                principalTable: "Ngos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObserver_MonitoringNgo_InviterNgoId",
                table: "MonitoringObserver",
                column: "InviterNgoId",
                principalTable: "MonitoringNgo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObserver_Observers_ObserverId",
                table: "MonitoringObserver",
                column: "ObserverId",
                principalTable: "Observers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObserverNotification_MonitoringObserver_TargetedO~",
                table: "MonitoringObserverNotification",
                column: "TargetedObserversId",
                principalTable: "MonitoringObserver",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationAttachment_MonitoringObserver_MonitoringObser~",
                table: "PollingStationAttachment",
                column: "MonitoringObserverId",
                principalTable: "MonitoringObserver",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationInformations_MonitoringObserver_MonitoringObs~",
                table: "PollingStationInformations",
                column: "MonitoringObserverId",
                principalTable: "MonitoringObserver",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationNote_MonitoringObserver_MonitoringObserverId",
                table: "PollingStationNote",
                column: "MonitoringObserverId",
                principalTable: "MonitoringObserver",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
