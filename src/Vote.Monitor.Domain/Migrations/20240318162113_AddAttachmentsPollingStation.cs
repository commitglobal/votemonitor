using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentsPollingStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PollingStationId",
                table: "PollingStationAttachment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationAttachment_PollingStationId",
                table: "PollingStationAttachment",
                column: "PollingStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PollingStationAttachment_PollingStations_PollingStationId",
                table: "PollingStationAttachment",
                column: "PollingStationId",
                principalTable: "PollingStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollingStationAttachment_PollingStations_PollingStationId",
                table: "PollingStationAttachment");

            migrationBuilder.DropIndex(
                name: "IX_PollingStationAttachment_PollingStationId",
                table: "PollingStationAttachment");

            migrationBuilder.DropColumn(
                name: "PollingStationId",
                table: "PollingStationAttachment");
        }
    }
}
