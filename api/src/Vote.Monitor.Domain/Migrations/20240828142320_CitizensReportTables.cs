using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CitizensReportTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuickReportAttachments_Id",
                table: "QuickReportAttachments");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedFileName",
                table: "QuickReportAttachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                table: "QuickReportAttachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "QuickReportAttachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "QuickReportAttachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<bool>(
                name: "AllowCitizenReporting",
                table: "ElectionRounds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "MonitoringNgoForCitizenReportingId",
                table: "ElectionRounds",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CitizenReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumberOfQuestionsAnswered = table.Column<int>(type: "integer", nullable: false),
                    NumberOfFlaggedAnswers = table.Column<int>(type: "integer", nullable: false),
                    FollowUpStatus = table.Column<string>(type: "text", nullable: false, defaultValue: "NotApplicable"),
                    Answers = table.Column<string>(type: "jsonb", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ContactInformation = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitizenReports_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitizenReports_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CitizenReportAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    CitizenReportId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_CitizenReportAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitizenReportAttachments_CitizenReports_CitizenReportId",
                        column: x => x.CitizenReportId,
                        principalTable: "CitizenReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitizenReportAttachments_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitizenReportAttachments_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CitizenReportNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<Guid>(type: "uuid", nullable: false),
                    CitizenReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenReportNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitizenReportNotes_CitizenReports_CitizenReportId",
                        column: x => x.CitizenReportId,
                        principalTable: "CitizenReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitizenReportNotes_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitizenReportNotes_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElectionRounds_MonitoringNgoForCitizenReportingId",
                table: "ElectionRounds",
                column: "MonitoringNgoForCitizenReportingId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReportAttachments_CitizenReportId",
                table: "CitizenReportAttachments",
                column: "CitizenReportId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReportAttachments_ElectionRoundId",
                table: "CitizenReportAttachments",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReportAttachments_FormId",
                table: "CitizenReportAttachments",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReportNotes_CitizenReportId",
                table: "CitizenReportNotes",
                column: "CitizenReportId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReportNotes_ElectionRoundId",
                table: "CitizenReportNotes",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReportNotes_FormId",
                table: "CitizenReportNotes",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReports_ElectionRoundId_FormId",
                table: "CitizenReports",
                columns: new[] { "ElectionRoundId", "FormId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReports_FormId",
                table: "CitizenReports",
                column: "FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElectionRounds_MonitoringNgos_MonitoringNgoForCitizenReport~",
                table: "ElectionRounds",
                column: "MonitoringNgoForCitizenReportingId",
                principalTable: "MonitoringNgos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElectionRounds_MonitoringNgos_MonitoringNgoForCitizenReport~",
                table: "ElectionRounds");

            migrationBuilder.DropTable(
                name: "CitizenReportAttachments");

            migrationBuilder.DropTable(
                name: "CitizenReportNotes");

            migrationBuilder.DropTable(
                name: "CitizenReports");

            migrationBuilder.DropIndex(
                name: "IX_ElectionRounds_MonitoringNgoForCitizenReportingId",
                table: "ElectionRounds");

            migrationBuilder.DropColumn(
                name: "AllowCitizenReporting",
                table: "ElectionRounds");

            migrationBuilder.DropColumn(
                name: "MonitoringNgoForCitizenReportingId",
                table: "ElectionRounds");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedFileName",
                table: "QuickReportAttachments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                table: "QuickReportAttachments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "QuickReportAttachments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "QuickReportAttachments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_QuickReportAttachments_Id",
                table: "QuickReportAttachments",
                column: "Id");
        }
    }
}
