using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObservers_MonitoringNgos_InviterNgoId",
                table: "MonitoringObservers");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObservers_MonitoringNgos_MonitoringNgoId",
                table: "MonitoringObservers");

            migrationBuilder.DropIndex(
                name: "IX_MonitoringObservers_InviterNgoId",
                table: "MonitoringObservers");

            migrationBuilder.DropColumn(
                name: "InviterNgoId",
                table: "MonitoringObservers");

            migrationBuilder.AlterColumn<Guid>(
                name: "MonitoringNgoId",
                table: "MonitoringObservers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObservers_MonitoringNgos_MonitoringNgoId",
                table: "MonitoringObservers",
                column: "MonitoringNgoId",
                principalTable: "MonitoringNgos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObservers_MonitoringNgos_MonitoringNgoId",
                table: "MonitoringObservers");

            migrationBuilder.AlterColumn<Guid>(
                name: "MonitoringNgoId",
                table: "MonitoringObservers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "InviterNgoId",
                table: "MonitoringObservers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringObservers_InviterNgoId",
                table: "MonitoringObservers",
                column: "InviterNgoId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObservers_MonitoringNgos_InviterNgoId",
                table: "MonitoringObservers",
                column: "InviterNgoId",
                principalTable: "MonitoringNgos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObservers_MonitoringNgos_MonitoringNgoId",
                table: "MonitoringObservers",
                column: "MonitoringNgoId",
                principalTable: "MonitoringNgos",
                principalColumn: "Id");
        }
    }
}
