using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FormType",
                table: "FormTemplates",
                newName: "FormTemplateType");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "NotificationToken",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "PollingStationAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    Filename = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationAttachment_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationAttachment_MonitoringObserver_MonitoringObser~",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObserver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationNote_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationNote_MonitoringObserver_MonitoringObserverId",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObserver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationNote_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationAttachment_ElectionRoundId",
                table: "PollingStationAttachment",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationAttachment_MonitoringObserverId",
                table: "PollingStationAttachment",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationNote_ElectionRoundId",
                table: "PollingStationNote",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationNote_MonitoringObserverId",
                table: "PollingStationNote",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationNote_PollingStationId",
                table: "PollingStationNote",
                column: "PollingStationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollingStationAttachment");

            migrationBuilder.DropTable(
                name: "PollingStationNote");

            migrationBuilder.RenameColumn(
                name: "FormTemplateType",
                table: "FormTemplates",
                newName: "FormType");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "NotificationToken",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);
        }
    }
}
