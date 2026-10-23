using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddAutomationRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "AssignedBy",
                table: "Reviewers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ActorName",
                table: "ReviewActivities",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<Guid>(
                name: "ActorId",
                table: "ReviewActivities",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "RuleId",
                table: "ReviewActivities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RuleKind",
                table: "ReviewActivities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RuleName",
                table: "ReviewActivities",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ChangedBy",
                table: "RecordSeverities",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "ReviewerRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StageIds = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    ReviewerIds = table.Column<Guid[]>(type: "uuid[]", nullable: false),
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
                    table.PrimaryKey("PK_ReviewerRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewerRules_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewerRules_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeverityRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_SeverityRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeverityRules_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeverityRules_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewerRules_ElectionRoundId",
                table: "ReviewerRules",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewerRules_MonitoringNgoId",
                table: "ReviewerRules",
                column: "MonitoringNgoId");

            migrationBuilder.CreateIndex(
                name: "IX_SeverityRules_ElectionRoundId",
                table: "SeverityRules",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_SeverityRules_MonitoringNgoId",
                table: "SeverityRules",
                column: "MonitoringNgoId");

            // Rule names are unique per monitoring NGO and kind, ignoring case (EF can't express this index).
            migrationBuilder.Sql(
                """CREATE UNIQUE INDEX "IX_ReviewerRules_MonitoringNgoId_Name" ON "ReviewerRules" ("MonitoringNgoId", lower("Name"));""");
            migrationBuilder.Sql(
                """CREATE UNIQUE INDEX "IX_SeverityRules_MonitoringNgoId_Name" ON "SeverityRules" ("MonitoringNgoId", lower("Name"));""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewerRules");

            migrationBuilder.DropTable(
                name: "SeverityRules");

            migrationBuilder.DropColumn(
                name: "RuleId",
                table: "ReviewActivities");

            migrationBuilder.DropColumn(
                name: "RuleKind",
                table: "ReviewActivities");

            migrationBuilder.DropColumn(
                name: "RuleName",
                table: "ReviewActivities");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssignedBy",
                table: "Reviewers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActorName",
                table: "ReviewActivities",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ActorId",
                table: "ReviewActivities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ChangedBy",
                table: "RecordSeverities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
