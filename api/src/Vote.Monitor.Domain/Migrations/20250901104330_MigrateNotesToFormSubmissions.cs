using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class MigrateNotesToFormSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubmissionId",
                table: "Notes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("""
                                 UPDATE "Notes" n
                                 SET "SubmissionId" = fs."Id"
                                 FROM "FormSubmissions" fs
                                 WHERE fs."FormId" = n."FormId"
                                   AND fs."ElectionRoundId" = n."ElectionRoundId"
                                   AND fs."PollingStationId" = n."PollingStationId"
                                   AND fs."MonitoringObserverId" = n."MonitoringObserverId"
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "Notes");
        }
    }
}
