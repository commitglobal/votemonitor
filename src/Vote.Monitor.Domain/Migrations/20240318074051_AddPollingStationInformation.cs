using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddPollingStationInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PollingStationInformationForms_ElectionRoundId",
                table: "PollingStationInformationForms");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformationForms_ElectionRoundId",
                table: "PollingStationInformationForms",
                column: "ElectionRoundId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PollingStationInformationForms_ElectionRoundId",
                table: "PollingStationInformationForms");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformationForms_ElectionRoundId",
                table: "PollingStationInformationForms",
                column: "ElectionRoundId");
        }
    }
}
