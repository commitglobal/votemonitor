using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ReworkAttachmentsAndNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollingStationAttachments");

            migrationBuilder.DropTable(
                name: "PollingStationNotes");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfQuestions",
                table: "FormTemplates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfQuestions",
                table: "Forms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_MonitoringObservers_MonitoringObserverId",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_MonitoringObservers_MonitoringObserverId",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ElectionRoundId",
                table: "Attachments",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_FormId",
                table: "Attachments",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_MonitoringObserverId",
                table: "Attachments",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_PollingStationId",
                table: "Attachments",
                column: "PollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_ElectionRoundId",
                table: "Notes",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_FormId",
                table: "Notes",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_MonitoringObserverId",
                table: "Notes",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_PollingStationId",
                table: "Notes",
                column: "PollingStationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropColumn(
                name: "NumberOfQuestions",
                table: "FormTemplates");

            migrationBuilder.DropColumn(
                name: "NumberOfQuestions",
                table: "Forms");

            migrationBuilder.CreateTable(
                name: "PollingStationAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MimeType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UploadedFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationAttachments_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationAttachments_MonitoringObservers_MonitoringObs~",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationAttachments_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationNotes_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationNotes_MonitoringObservers_MonitoringObserverId",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationNotes_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationAttachments_ElectionRoundId",
                table: "PollingStationAttachments",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationAttachments_MonitoringObserverId",
                table: "PollingStationAttachments",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationAttachments_PollingStationId",
                table: "PollingStationAttachments",
                column: "PollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationNotes_ElectionRoundId",
                table: "PollingStationNotes",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationNotes_MonitoringObserverId",
                table: "PollingStationNotes",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationNotes_PollingStationId",
                table: "PollingStationNotes",
                column: "PollingStationId");
        }
    }
}
