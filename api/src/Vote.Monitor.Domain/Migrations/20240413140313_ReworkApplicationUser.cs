using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ReworkApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Observers_ApplicationUserId",
                table: "Observers");

            migrationBuilder.DropIndex(
                name: "IX_NgoAdmins_ApplicationUserId",
                table: "NgoAdmins");

            migrationBuilder.AddColumn<Guid>(
                name: "ElectionRoundId",
                table: "MonitoringObservers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InvitationToken",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Observers_ApplicationUserId",
                table: "Observers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NgoAdmins_ApplicationUserId",
                table: "NgoAdmins",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringObservers_ElectionRoundId_Id",
                table: "MonitoringObservers",
                columns: new[] { "ElectionRoundId", "Id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObservers_ElectionRounds_ElectionRoundId",
                table: "MonitoringObservers",
                column: "ElectionRoundId",
                principalTable: "ElectionRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObservers_ElectionRounds_ElectionRoundId",
                table: "MonitoringObservers");

            migrationBuilder.DropIndex(
                name: "IX_Observers_ApplicationUserId",
                table: "Observers");

            migrationBuilder.DropIndex(
                name: "IX_NgoAdmins_ApplicationUserId",
                table: "NgoAdmins");

            migrationBuilder.DropIndex(
                name: "IX_MonitoringObservers_ElectionRoundId_Id",
                table: "MonitoringObservers");

            migrationBuilder.DropColumn(
                name: "ElectionRoundId",
                table: "MonitoringObservers");

            migrationBuilder.DropColumn(
                name: "InvitationToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Observers_ApplicationUserId",
                table: "Observers",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NgoAdmins_ApplicationUserId",
                table: "NgoAdmins",
                column: "ApplicationUserId",
                unique: true);
        }
    }
}
