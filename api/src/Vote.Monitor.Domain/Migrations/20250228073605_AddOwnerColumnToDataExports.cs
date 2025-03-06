using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerColumnToDataExports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "ExportedData",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ExportedData_OwnerId",
                table: "ExportedData",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportedData_AspNetUsers_OwnerId",
                table: "ExportedData",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportedData_AspNetUsers_OwnerId",
                table: "ExportedData");

            migrationBuilder.DropIndex(
                name: "IX_ExportedData_OwnerId",
                table: "ExportedData");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ExportedData");
        }
    }
}
