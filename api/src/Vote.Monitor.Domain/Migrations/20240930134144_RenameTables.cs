using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueReportAttachments");

            migrationBuilder.DropTable(
                name: "IssueReportNotes");

            migrationBuilder.DropTable(
                name: "IssueReports");

            migrationBuilder.CreateTable(
                name: "IncidentReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationType = table.Column<string>(type: "text", nullable: false),
                    LocationDescription = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: true),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumberOfQuestionsAnswered = table.Column<int>(type: "integer", nullable: false),
                    NumberOfFlaggedAnswers = table.Column<int>(type: "integer", nullable: false),
                    FollowUpStatus = table.Column<string>(type: "text", nullable: false, defaultValue: "NotApplicable"),
                    Answers = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentReports_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReports_MonitoringObservers_MonitoringObserverId",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReports_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IncidentReportAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    IncidentReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UploadedFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReportAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentReportAttachments_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReportAttachments_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReportAttachments_IncidentReports_IncidentReportId",
                        column: x => x.IncidentReportId,
                        principalTable: "IncidentReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncidentReportNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false),
                    IncidentReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReportNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentReportNotes_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReportNotes_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReportNotes_IncidentReports_IncidentReportId",
                        column: x => x.IncidentReportId,
                        principalTable: "IncidentReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReportAttachments_ElectionRoundId",
                table: "IncidentReportAttachments",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReportAttachments_FormId",
                table: "IncidentReportAttachments",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReportAttachments_IncidentReportId",
                table: "IncidentReportAttachments",
                column: "IncidentReportId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReportNotes_ElectionRoundId",
                table: "IncidentReportNotes",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReportNotes_FormId",
                table: "IncidentReportNotes",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReportNotes_IncidentReportId",
                table: "IncidentReportNotes",
                column: "IncidentReportId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_ElectionRoundId",
                table: "IncidentReports",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_FormId",
                table: "IncidentReports",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_MonitoringObserverId",
                table: "IncidentReports",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_PollingStationId",
                table: "IncidentReports",
                column: "PollingStationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentReportAttachments");

            migrationBuilder.DropTable(
                name: "IncidentReportNotes");

            migrationBuilder.DropTable(
                name: "IncidentReports");

            migrationBuilder.CreateTable(
                name: "IssueReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Answers = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FollowUpStatus = table.Column<string>(type: "text", nullable: false, defaultValue: "NotApplicable"),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LocationDescription = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    LocationType = table.Column<string>(type: "text", nullable: false),
                    NumberOfFlaggedAnswers = table.Column<int>(type: "integer", nullable: false),
                    NumberOfQuestionsAnswered = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueReports_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueReports_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueReports_MonitoringObservers_MonitoringObserverId",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueReports_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IssueReportAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MimeType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UploadedFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueReportAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueReportAttachments_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueReportAttachments_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueReportAttachments_IssueReports_IssueReportId",
                        column: x => x.IssueReportId,
                        principalTable: "IssueReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueReportNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueReportNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueReportNotes_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueReportNotes_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueReportNotes_IssueReports_IssueReportId",
                        column: x => x.IssueReportId,
                        principalTable: "IssueReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssueReportAttachments_ElectionRoundId",
                table: "IssueReportAttachments",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReportAttachments_FormId",
                table: "IssueReportAttachments",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReportAttachments_IssueReportId",
                table: "IssueReportAttachments",
                column: "IssueReportId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReportNotes_ElectionRoundId",
                table: "IssueReportNotes",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReportNotes_FormId",
                table: "IssueReportNotes",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReportNotes_IssueReportId",
                table: "IssueReportNotes",
                column: "IssueReportId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_ElectionRoundId",
                table: "IssueReports",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_FormId",
                table: "IssueReports",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_MonitoringObserverId",
                table: "IssueReports",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_PollingStationId",
                table: "IssueReports",
                column: "PollingStationId");
        }
    }
}
