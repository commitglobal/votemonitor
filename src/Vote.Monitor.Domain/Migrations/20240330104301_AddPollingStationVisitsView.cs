using Microsoft.EntityFrameworkCore.Migrations;
using Vote.Monitor.Domain.Constants;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddPollingStationVisitsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
              CREATE OR REPLACE VIEW ""{Views.PollingStationVisits}"" AS 
              SELECT t.""ElectionRoundId"", 
              t.""PollingStationId"",
              mo.""MonitoringNgoId"",
              mn.""NgoId"",
              t.""MonitoringObserverId"",
              mo.""ObserverId"",
              MIN(t.""LatestTimestamp"") ""VisitedAt""
              FROM (
                  SELECT psi.""ElectionRoundId"",
                  psi.""PollingStationId"",
                  psi.""MonitoringObserverId"",
                  COALESCE(psi.""ArrivalTime"", psi.""LastModifiedOn"", psi.""CreatedOn"") ""LatestTimestamp""
                  FROM ""{Tables.PollingStationInformation}"" psi
                  UNION
                  SELECT
                  n.""ElectionRoundId"", 
                  n.""PollingStationId"", 
                  n.""MonitoringObserverId"", 
                  COALESCE(n.""LastModifiedOn"", n.""CreatedOn"") ""LatestTimestamp""
                  FROM ""{Tables.PollingStationNotes}"" n
                  UNION
                  SELECT
                  a.""ElectionRoundId"", 
                  a.""PollingStationId"", 
                  a.""MonitoringObserverId"", 
                  COALESCE(a.""LastModifiedOn"", a.""CreatedOn"") ""LatestTimestamp""
                  FROM ""{Tables.PollingStationAttachments}"" a
              ) t 
              INNER JOIN ""MonitoringObservers"" mo ON mo.""Id"" = t.""MonitoringObserverId""
              INNER JOIN ""MonitoringNgos"" mn ON mo.""MonitoringNgoId"" = mn.""Id""
              GROUP BY  t.""ElectionRoundId"", t.""PollingStationId"", mo.""MonitoringNgoId"", mn.""NgoId"", t.""MonitoringObserverId"", mo.""ObserverId"";
              ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP VIEW IF EXISTS ""{Views.PollingStationVisits}""");
        }
    }
}
