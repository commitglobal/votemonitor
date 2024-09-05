using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCitizenReportIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CitizenReports_ElectionRoundId_FormId",
                table: "CitizenReports");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReports_ElectionRoundId",
                table: "CitizenReports",
                column: "ElectionRoundId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CitizenReports_ElectionRoundId",
                table: "CitizenReports");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReports_ElectionRoundId_FormId",
                table: "CitizenReports",
                columns: new[] { "ElectionRoundId", "FormId" },
                unique: true);
        }
    }
}
