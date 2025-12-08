using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class MigrateAttachmentsToFormSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubmissionId",
                table: "Attachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
            
            
            migrationBuilder.Sql("""
                                 UPDATE "Attachments" a
                                 SET "SubmissionId" = fs."Id"
                                 FROM "FormSubmissions" fs
                                 WHERE fs."FormId" = a."FormId"
                                   AND fs."ElectionRoundId" = a."ElectionRoundId"
                                   AND fs."PollingStationId" = a."PollingStationId"
                                   AND fs."MonitoringObserverId" = a."MonitoringObserverId"
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "Attachments");
        }
    }
}
