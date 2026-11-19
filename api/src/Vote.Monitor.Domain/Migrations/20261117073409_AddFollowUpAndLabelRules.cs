using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <summary>
    /// Two more kinds of automation rule: follow-up rules (set a record's follow-up status while it still has the NGO's
    /// default) and label rules (add labels). Names are unique per monitoring NGO and kind, ignoring case, like the
    /// reviewer and severity rules'.
    /// </summary>
    public partial class AddFollowUpAndLabelRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FollowUpRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowUpStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RecordTypes = table.Column<string[]>(type: "text[]", nullable: false),
                    Conditions = table.Column<JsonDocument>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowUpRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowUpRules_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowUpRules_FollowUpStatuses_FollowUpStatusId",
                        column: x => x.FollowUpStatusId,
                        principalTable: "FollowUpStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowUpRules_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabelRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Labels = table.Column<string[]>(type: "text[]", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RecordTypes = table.Column<string[]>(type: "text[]", nullable: false),
                    Conditions = table.Column<JsonDocument>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabelRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabelRules_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabelRules_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpRules_ElectionRoundId",
                table: "FollowUpRules",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpRules_FollowUpStatusId",
                table: "FollowUpRules",
                column: "FollowUpStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpRules_MonitoringNgoId",
                table: "FollowUpRules",
                column: "MonitoringNgoId");

            migrationBuilder.CreateIndex(
                name: "IX_LabelRules_ElectionRoundId",
                table: "LabelRules",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_LabelRules_MonitoringNgoId",
                table: "LabelRules",
                column: "MonitoringNgoId");

            migrationBuilder.Sql(
                """CREATE UNIQUE INDEX "IX_FollowUpRules_MonitoringNgoId_Name" ON "FollowUpRules" ("MonitoringNgoId", lower("Name"));""");
            migrationBuilder.Sql(
                """CREATE UNIQUE INDEX "IX_LabelRules_MonitoringNgoId_Name" ON "LabelRules" ("MonitoringNgoId", lower("Name"));""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FollowUpRules");

            migrationBuilder.DropTable(
                name: "LabelRules");
        }
    }
}
