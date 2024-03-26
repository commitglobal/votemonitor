using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ObserversGuide : Migration
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

            migrationBuilder.AddColumn<Guid>(
                name: "MonitoringNgoId",
                table: "MonitoringObserver",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ObserverGuide",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UploadedFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObserverGuide", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObserverGuide_MonitoringNgo_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationInformationForms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    Languages = table.Column<string>(type: "jsonb", nullable: false),
                    Questions = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationInformationForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationInformationForms_ElectionRounds_ElectionRound~",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationInformations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationInformationFormId = table.Column<Guid>(type: "uuid", nullable: false),
                    Answers = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationInformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_MonitoringObserver_MonitoringObs~",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObserver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_PollingStationInformationForms_P~",
                        column: x => x.PollingStationInformationFormId,
                        principalTable: "PollingStationInformationForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollingStations_ElectionRoundId",
                table: "PollingStations",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringObserver_MonitoringNgoId",
                table: "MonitoringObserver",
                column: "MonitoringNgoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObserverGuide_MonitoringNgoId",
                table: "ObserverGuide",
                column: "MonitoringNgoId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformationForms_ElectionRoundId",
                table: "PollingStationInformationForms",
                column: "ElectionRoundId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_ElectionRoundId",
                table: "PollingStationInformations",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_MonitoringObserverId",
                table: "PollingStationInformations",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_PollingStationId",
                table: "PollingStationInformations",
                column: "PollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_PollingStationInformationFormId",
                table: "PollingStationInformations",
                column: "PollingStationInformationFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObserver_MonitoringNgo_MonitoringNgoId",
                table: "MonitoringObserver",
                column: "MonitoringNgoId",
                principalTable: "MonitoringNgo",
                principalColumn: "Id");

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
                name: "FK_MonitoringObserver_MonitoringNgo_MonitoringNgoId",
                table: "MonitoringObserver");

            migrationBuilder.DropForeignKey(
                name: "FK_PollingStations_ElectionRounds_ElectionRoundId",
                table: "PollingStations");

            migrationBuilder.DropTable(
                name: "ObserverGuide");

            migrationBuilder.DropTable(
                name: "PollingStationInformations");

            migrationBuilder.DropTable(
                name: "PollingStationInformationForms");

            migrationBuilder.DropIndex(
                name: "IX_PollingStations_ElectionRoundId",
                table: "PollingStations");

            migrationBuilder.DropIndex(
                name: "IX_MonitoringObserver_MonitoringNgoId",
                table: "MonitoringObserver");

            migrationBuilder.DropColumn(
                name: "ElectionRoundId",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "MonitoringNgoId",
                table: "MonitoringObserver");

            migrationBuilder.RenameColumn(
                name: "Questions",
                table: "FormTemplates",
                newName: "Sections");
        }
    }
}
