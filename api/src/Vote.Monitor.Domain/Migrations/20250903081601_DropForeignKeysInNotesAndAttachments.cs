using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_ElectionRounds_ElectionRoundId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Forms_FormId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_PollingStations_PollingStationId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_ElectionRounds_ElectionRoundId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Forms_FormId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_PollingStations_PollingStationId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_ElectionRoundId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_FormId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_PollingStationId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_ElectionRoundId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_FormId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_PollingStationId",
                table: "Attachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notes_ElectionRoundId",
                table: "Notes",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_FormId",
                table: "Notes",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_PollingStationId",
                table: "Notes",
                column: "PollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ElectionRoundId",
                table: "Attachments",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_FormId",
                table: "Attachments",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_PollingStationId",
                table: "Attachments",
                column: "PollingStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_ElectionRounds_ElectionRoundId",
                table: "Attachments",
                column: "ElectionRoundId",
                principalTable: "ElectionRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Forms_FormId",
                table: "Attachments",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_PollingStations_PollingStationId",
                table: "Attachments",
                column: "PollingStationId",
                principalTable: "PollingStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_ElectionRounds_ElectionRoundId",
                table: "Notes",
                column: "ElectionRoundId",
                principalTable: "ElectionRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Forms_FormId",
                table: "Notes",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_PollingStations_PollingStationId",
                table: "Notes",
                column: "PollingStationId",
                principalTable: "PollingStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
