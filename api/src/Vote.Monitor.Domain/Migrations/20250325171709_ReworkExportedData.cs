using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ReworkExportedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportedData_ElectionRounds_ElectionRoundId",
                table: "ExportedData");

            migrationBuilder.DropIndex(
                name: "IX_ExportedData_ElectionRoundId",
                table: "ExportedData");

            migrationBuilder.DropColumn(
                name: "ElectionRoundId",
                table: "ExportedData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ElectionRoundId",
                table: "ExportedData",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ExportedData_ElectionRoundId",
                table: "ExportedData",
                column: "ElectionRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportedData_ElectionRounds_ElectionRoundId",
                table: "ExportedData",
                column: "ElectionRoundId",
                principalTable: "ElectionRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
