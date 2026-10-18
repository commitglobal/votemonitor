using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameIncidentReportOtherLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 UPDATE "IncidentReports"
                                 SET "LocationType" = 'NotRelatedToAPollingStation'
                                 WHERE "LocationType" = 'OtherLocation';
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // OtherPollingStation did not exist before, so those reports go back to OtherLocation as well.
            migrationBuilder.Sql("""
                                 UPDATE "IncidentReports"
                                 SET "LocationType" = 'OtherLocation'
                                 WHERE "LocationType" IN ('NotRelatedToAPollingStation', 'OtherPollingStation');
                                 """);
        }
    }
}
