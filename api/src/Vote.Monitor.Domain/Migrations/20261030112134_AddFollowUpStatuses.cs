using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowUpStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FollowUpStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Sequence = table.Column<double>(type: "double precision", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Icon = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LegacyStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowUpStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowUpStatuses_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecordFollowUps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordType = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowUpStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordFollowUps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordFollowUps_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordFollowUps_FollowUpStatuses_FollowUpStatusId",
                        column: x => x.FollowUpStatusId,
                        principalTable: "FollowUpStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordFollowUps_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpStatuses_MonitoringNgoId_Default",
                table: "FollowUpStatuses",
                column: "MonitoringNgoId",
                unique: true,
                filter: "\"IsDefault\"");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpStatuses_MonitoringNgoId_Name",
                table: "FollowUpStatuses",
                columns: new[] { "MonitoringNgoId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecordFollowUps_ElectionRoundId",
                table: "RecordFollowUps",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordFollowUps_FollowUpStatusId",
                table: "RecordFollowUps",
                column: "FollowUpStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordFollowUps_MonitoringNgoId_RecordType_RecordId",
                table: "RecordFollowUps",
                columns: new[] { "MonitoringNgoId", "RecordType", "RecordId" },
                unique: true);

            // Every existing monitoring NGO starts with the default follow-up statuses (FollowUpStatusDefinition.CreateDefaults).
            migrationBuilder.Sql("""
                INSERT INTO "FollowUpStatuses" ("Id", "MonitoringNgoId", "Name", "Description", "Sequence", "IsDefault", "Color", "Icon", "LegacyStatus", "CreatedOn", "CreatedBy", "LastModifiedBy")
                SELECT gen_random_uuid(), MN."Id", S."Name", NULL, S."Sequence", S."IsDefault", S."Color", S."Icon", S."LegacyStatus", now() AT TIME ZONE 'UTC', MN."CreatedBy", MN."CreatedBy"
                FROM "MonitoringNgos" MN
                CROSS JOIN (VALUES
                    ('Not applicable', 1000, TRUE, 'gray', 'circle', 'NotApplicable'),
                    ('Needs follow-up', 2000, FALSE, 'yellow', 'alert', 'NeedsFollowUp'),
                    ('Resolved', 3000, FALSE, 'green', 'check', 'Resolved')
                ) AS S("Name", "Sequence", "IsDefault", "Color", "Icon", "LegacyStatus");
                """);

            // The follow-up each record had becomes its own NGO's (the NGO that could set it: the observer's, or the
            // form's for incident and citizen reports). Not applicable is every NGO's default, so it needs no row.
            migrationBuilder.Sql("""
                INSERT INTO "RecordFollowUps" ("Id", "ElectionRoundId", "MonitoringNgoId", "RecordType", "RecordId", "FollowUpStatusId", "ChangedAt", "CreatedOn", "CreatedBy", "LastModifiedBy")
                SELECT gen_random_uuid(), R."ElectionRoundId", R."MonitoringNgoId", R."RecordType", R."Id", FUS."Id", now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000'
                FROM (
                    SELECT FS."ElectionRoundId", MO."MonitoringNgoId", 'FormSubmission' AS "RecordType", FS."Id", FS."FollowUpStatus"
                    FROM "FormSubmissions" FS JOIN "MonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                    UNION ALL
                    SELECT PSI."ElectionRoundId", MO."MonitoringNgoId", 'FormSubmission', PSI."Id", PSI."FollowUpStatus"
                    FROM "PollingStationInformation" PSI JOIN "MonitoringObservers" MO ON MO."Id" = PSI."MonitoringObserverId"
                    UNION ALL
                    SELECT QR."ElectionRoundId", MO."MonitoringNgoId", 'QuickReport', QR."Id", QR."FollowUpStatus"
                    FROM "QuickReports" QR JOIN "MonitoringObservers" MO ON MO."Id" = QR."MonitoringObserverId"
                    UNION ALL
                    SELECT IR."ElectionRoundId", F."MonitoringNgoId", 'IncidentReport', IR."Id", IR."FollowUpStatus"
                    FROM "IncidentReports" IR JOIN "Forms" F ON F."Id" = IR."FormId"
                    UNION ALL
                    SELECT CR."ElectionRoundId", F."MonitoringNgoId", 'CitizenReport', CR."Id", CR."FollowUpStatus"
                    FROM "CitizenReports" CR JOIN "Forms" F ON F."Id" = CR."FormId"
                ) R
                JOIN "FollowUpStatuses" FUS ON FUS."MonitoringNgoId" = R."MonitoringNgoId" AND FUS."LegacyStatus" = R."FollowUpStatus"
                WHERE R."FollowUpStatus" <> 'NotApplicable'
                ON CONFLICT ("MonitoringNgoId", "RecordType", "RecordId") DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordFollowUps");

            migrationBuilder.DropTable(
                name: "FollowUpStatuses");
        }
    }
}
