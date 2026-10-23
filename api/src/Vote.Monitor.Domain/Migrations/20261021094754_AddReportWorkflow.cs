using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddReportWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordType = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "text", nullable: false),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FromStageId = table.Column<Guid>(type: "uuid", nullable: true),
                    FromStageName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ToStageId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToStageName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ToStatus = table.Column<string>(type: "text", nullable: true),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewerName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FollowUpStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewActivities_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewActivities_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviewers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordType = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviewers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviewers_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviewers_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewStages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Sequence = table.Column<double>(type: "double precision", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewStages_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordType = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotifications_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotifications_ReviewActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "ReviewActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewWorkflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordType = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    StageId = table.Column<Guid>(type: "uuid", nullable: false),
                    StageChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewWorkflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewWorkflows_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewWorkflows_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewWorkflows_ReviewStages_StageId",
                        column: x => x.StageId,
                        principalTable: "ReviewStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewActivities_ElectionRoundId",
                table: "ReviewActivities",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewActivities_MonitoringNgoId_RecordType_RecordId_Occurr~",
                table: "ReviewActivities",
                columns: new[] { "MonitoringNgoId", "RecordType", "RecordId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_ElectionRoundId_UserId",
                table: "Reviewers",
                columns: new[] { "ElectionRoundId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_MonitoringNgoId_RecordType_RecordId_UserId",
                table: "Reviewers",
                columns: new[] { "MonitoringNgoId", "RecordType", "RecordId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_UserId",
                table: "Reviewers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStages_MonitoringNgoId_Default",
                table: "ReviewStages",
                column: "MonitoringNgoId",
                unique: true,
                filter: "\"IsDefault\"");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStages_MonitoringNgoId_Name",
                table: "ReviewStages",
                columns: new[] { "MonitoringNgoId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewWorkflows_ElectionRoundId",
                table: "ReviewWorkflows",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewWorkflows_MonitoringNgoId_RecordType_RecordId",
                table: "ReviewWorkflows",
                columns: new[] { "MonitoringNgoId", "RecordType", "RecordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewWorkflows_StageId",
                table: "ReviewWorkflows",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_ActivityId",
                table: "UserNotifications",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_ElectionRoundId",
                table: "UserNotifications",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId_ReadAt_CreatedOn",
                table: "UserNotifications",
                columns: new[] { "UserId", "ReadAt", "CreatedOn" });

            // Every existing monitoring NGO starts with the default review stages (ReviewStage.CreateDefaults).
            migrationBuilder.Sql("""
                INSERT INTO "ReviewStages" ("Id", "MonitoringNgoId", "Name", "Description", "Status", "Sequence", "IsDefault", "CreatedOn", "CreatedBy", "LastModifiedBy")
                SELECT gen_random_uuid(), MN."Id", S."Name", NULL, S."Status", 1000, S."IsDefault", now() AT TIME ZONE 'UTC', MN."CreatedBy", MN."CreatedBy"
                FROM "MonitoringNgos" MN
                CROSS JOIN (VALUES ('Not reviewed', 'NotReviewed', TRUE), ('In review', 'InReview', FALSE), ('Reviewed', 'Reviewed', FALSE)) AS S("Name", "Status", "IsDefault");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviewers");

            migrationBuilder.DropTable(
                name: "ReviewWorkflows");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "ReviewStages");

            migrationBuilder.DropTable(
                name: "ReviewActivities");
        }
    }
}
