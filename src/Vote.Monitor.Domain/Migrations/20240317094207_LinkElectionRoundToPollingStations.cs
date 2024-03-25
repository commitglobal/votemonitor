using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class LinkElectionRoundToPollingStations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sections",
                table: "FormTemplates",
                newName: "Questions");

            migrationBuilder.AddColumn<Guid>(
                name: "ElectionRoundId",
                table: "PollingStations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PollingStations_ElectionRoundId",
                table: "PollingStations",
                column: "ElectionRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStations_ElectionRounds_ElectionRoundId",
                table: "PollingStations",
                column: "ElectionRoundId",
                principalTable: "ElectionRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollingStations_ElectionRounds_ElectionRoundId",
                table: "PollingStations");

            migrationBuilder.DropIndex(
                name: "IX_PollingStations_ElectionRoundId",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "ElectionRoundId",
                table: "PollingStations");

            migrationBuilder.RenameColumn(
                name: "Questions",
                table: "FormTemplates",
                newName: "Sections");
        }
    }
}
