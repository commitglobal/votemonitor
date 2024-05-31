using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNgoIdFromExportJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportedData_Ngos_NgoId",
                table: "ExportedData");

            migrationBuilder.DropIndex(
                name: "IX_ExportedData_NgoId",
                table: "ExportedData");

            migrationBuilder.DropColumn(
                name: "NgoId",
                table: "ExportedData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NgoId",
                table: "ExportedData",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ExportedData_NgoId",
                table: "ExportedData",
                column: "NgoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportedData_Ngos_NgoId",
                table: "ExportedData",
                column: "NgoId",
                principalTable: "Ngos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
