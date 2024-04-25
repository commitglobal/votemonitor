using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddExportedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_ElectionRoundId",
                table: "FormSubmissions");

            migrationBuilder.CreateTable(
                name: "ExportedData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    NgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExportStatus = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Base64EncodedData = table.Column<string>(type: "text", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportedData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportedData_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExportedData_Ngos_NgoId",
                        column: x => x.NgoId,
                        principalTable: "Ngos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_ElectionRoundId_PollingStationId_Monitoring~",
                table: "FormSubmissions",
                columns: new[] { "ElectionRoundId", "PollingStationId", "MonitoringObserverId", "FormId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExportedData_ElectionRoundId",
                table: "ExportedData",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportedData_Id",
                table: "ExportedData",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExportedData_NgoId",
                table: "ExportedData",
                column: "NgoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportedData");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_ElectionRoundId_PollingStationId_Monitoring~",
                table: "FormSubmissions");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_ElectionRoundId",
                table: "FormSubmissions",
                column: "ElectionRoundId");
        }
    }
}
